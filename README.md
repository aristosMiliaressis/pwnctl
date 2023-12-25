PWNCTL ![ci: tag](https://github.com/aristosMiliaressis/pwnctl/actions/workflows/ci.yml/badge.svg)
==

**Table of Contents**
- [What is this?](#what-is-this)
- [How does it Work?](#how-does-it-work)</br>
  - [Assets & Tags](#assets--tags)</br>
  - [Scope](#scope)</br>
  - [Tasks & Notifications](#tasks--notifications)</br>
  - [Operations](#operations)
- [How to set it up?](#how-to-set-it-up)

## What is this?

a framework for external recon that facilitates discovering, scanning and monitoring assets trough a configurable engine running on serverless aws infrastructure.

## How does it Work?

<p align="center">
  <img src="https://github.com/aristosMiliaressis/pwnctl/blob/master/img/arch-phase1.png?raw=true">
</p>

tasks are placed in an sqs queue that causes container instances to spin up perform the recon tasks and process the output according to configuration.

### Assets & Tags

task output is processed line by line, each line is classified into one of the following asset classes.

| Asset            |   Notation                                       |
|------------------|:------------------------------------------------:|  
| DomainName       | example.com                                      |
| DomainNameRecord | example.com IN A 1.3.3.7                         |
| Email            | johndoe@example.com                              |
| HttpEndpoint     | https://example.com/api                          |
| HttpParameter    | https://example.com/api?token=                   |
| NetworkHost      | 1.3.3.7, FD00:DEAD:BEEF:64:35::2                 |
| NetworkRange     | 12.34.45.0/24, 2001:db8::/48                     |
| NetworkSocket    | 1.3.3.7:443, udp://1.3.3.7:161, [2001:db8::]:80  |

* a dns pseudo record is supported to describe virtual hosting (e.g. `example.com IN VHOST 1.2.3.4`)

lines can either be in "raw" or json format, json format suports arbitary key/value tags

```
example.com
example.com:443
{"Asset":"https://example.com/api","Tags":{"Server":"IIS"}}
sub.example.com
sub2.example.com
```

**Tags** are a way to store arbitary metadata relating to an asset, they can be used in the `Filter` field (trough an indexer on the asset base class) to chain tasks into workflows where one task (e.g nmap) discovers some metadata relating to an asset (e.g. IIS service banner) which than causes a specific task to be queued (e.g. IIS shortname scanning)

### Scope

scope is determined trough explicit matching of `ScopeDefinition` patterns or trough a set of implicit relationship rules.

scope definitions can be one of three types (`DomainRegex`, `UrlRegex` or `CIDR`), and can be grouped into ScopeAggregates

aside from direct matching assets will be considered inscope according to the following rules
- Domain Name Records belonging to in scope domain are inscope
- Ip addreses connected to inscope domain trough A/AAAA record are also inscope
- Domains connected to inscope domain trough CNAME record are NOT inscope
- NetworkSockets/HttpEndpoints/HttpParameters routing to inscope ips or domains are inscope
- Emails containing inscope domain are in scope

scope definitions & scope aggregates can be created trough the rest api & cli.

### Tasks & Notifications

tasks are configured trough TaskDefinitions and can be organized into TaskProfiles.

tasks definitions are specific to an assset class (i.e domain_resolution task is specific to assets of type `DomainName`)

assets can be filtered trough the CSharpScript `Filter` field that has access to the asset class and associated Tags.

task definitions & task profiles can be seeded trough yaml files or created trough the rest api & cli.

task definitions can have monitoring rules that are only evaluated for `Monitor` type operations.

[Default Task Configuration](https://github.com/aristosMiliaressis/pwnctl/blob/master/src/pwnctl.api/App_Data/seed/)

**`task-definitions.td.yml`**
```YAML
Profiles: [ "all" ]
TaskDefinitions:
  - Name: sub_enum
    CommandTemplate: sub-enum.sh {{Name}}
    Filter: DomainName.ZoneDepth == 1
    Subject: DomainName

  - Name: ping_sweep
    CommandTemplate: ping-sweep.sh {{CIDR}}
    Subject: NetworkRange
    MonitorRules:
      Schedule: 0 0 * * *

  - Name: reverse_range_lookup
    CommandTemplate: reverse-range-lookup.sh {{CIDR}}
    Subject: NetworkRange
    MonitorRules:
      Schedule: 0 0 * * *

  - Name: domain_resolution
    CommandTemplate: resolve.sh {{Name}}
    Subject: DomainName
    MonitorRules:
      Schedule: 0 0 * * *
      PostCondition: newTags["rcode"] != oldTags["rcode"]
      NotificationTemplate: domain {{Name}} changed rcode from {{oldTags["rcode"]}} to {{newTags["rcode"]}}
    ShortLived: true

  - Name: httpx
    CommandTemplate: echo {{Name}} | httpx -silent
    Subject: DomainName
    ShortLived: true

  - Name: grab_headers
    CommandTemplate: grab-headers.sh {{Url}}
    Filter: HttpEndpoint.Path == "/"
    Subject: HttpEndpoint
    MonitorRules:
      Schedule: 0 0 * * *
      PreCondition: new List<string>{ "401", "403", "404", "500", "501", "502", "503", "504" }.Contains(Tags["Status"])
      PostCondition: newTags["Status"] != oldTags["Status"]
      NotificationTemplate: Endpoint {{Url}} changed status code from {{oldTags["Status"]}} to {{newTags["Status"]}}
    ShortLived: true

  - Name: asn_lookup
    CommandTemplate: asn-lookup.sh {{IP}}
    Subject: NetworkHost
    ShortLived: true

  - Name: tcp_scan
    CommandTemplate: tcp-scan.sh {{IP}}
    Subject: NetworkHost

  - Name: udp_scan
    CommandTemplate: udp-scan.sh {{IP}}
    Subject: NetworkHost

  - Name: tls_probe
    CommandTemplate: tls-probe.sh {{Address}}
    Subject: NetworkSocket
    ShortLived: true

  - Name: waymore
    CommandTemplate: waymore.sh {{Url}}
    Subject: HttpEndpoint
    Filter: HttpEndpoint.Path == "/"

  - Name: file_brute_config
    CommandTemplate: file-brute.sh {{Url}} /opt/wordlists/config.txt
    Filter: HttpEndpoint.Path == "/"
    Subject: HttpEndpoint

  - Name: vhost_scan
    Subject: HttpEndpoint
    Filter: HttpEndpoint.Path == "/" && HttpEndpoint.IsIpBased
    CommandTemplate: vhost-scan.sh {{Url}}
    StdinQuery: SELECT "TextNotation" FROM "asset_records" WHERE "InScope" = true AND "DomainNameId" IS NOT NULL
```

tasks can be annotated as `shortlived` (those that take 2 minutes or less) which will place them in a separate task queue that is consumed from a service running on spot instances which gives those tasks a 95% cost reduction in terms of compute cost.

**Notification Configuration**

a notify `provider-config.yaml` file with valid discord configuration should be placed in the `deployment/config/notify/` directory to enable notifications.

1. worker instances send status notification at start up & shutdown
2. configurable notification rules with CSharpScript `Filter` field like `TaskDefinitions` 

notification rules can be seeded trough yaml files or created trough the rest api & cli.

**`notification-rules.yml`**
```YAML
- Name: default_creds
  Subject: NetworkSocket
  Filter: Tags["vuln-default-creds"] == "true"
  Topic: misconfigs
  
- Name: cors_misconfig
  Subject: HttpEndpoint
  Filter: Tags["cors-misconfig"] == "true"
  Topic: misconfigs

- Name: shortname_misconfig
  Subject: HttpEndpoint
  Filter: Tags["shortname-misconfig"] == "true"
  Topic: misconfigs

- Name: s3_misconfig
  Subject: DomainName
  Filter: Tags["s3-takeover"] == "true" || !string.IsNullOrEmpty(Tags["s3-public-perms"])
  Topic: misconfigs
  CheckOutOfScope: true

- Name: node_debug
  Subject: NetworkSocket
  Filter: NetworkSocket.Port == 9228 || NetworkSocket.Port == 9229
  Topic: misconfigs

- Name: docker_api
  Subject: NetworkSocket
  Filter: NetworkSocket.Port == 2735 || NetworkSocket.Port == 2736
  Topic: misconfigs
```

### Operations

there are three types of operations `Crawl`, `Scan` & `Monitor`.

every operation has an associated `ScopeAggregate` and  a list of `TaskProfiles`.

**Crawl** operations have an addition Input list that contains the initial assets that should be processed to start a recursive loop where tasks will discover new assets and further tasks will be queued for the new assets to keep the loop going untill all the discoverable scope has been crawled according to the `TaskProfile` configuration.

**Scan** operations can be used to expand on the collected assets or discover misconfigurations in a controlled manner where pre-discovered assets will get assigned tasks but outputs will not be recursivly processed like in crawl mode.

**Monitor** operations allow you to periodicly monitor assets for change, `Monitor` operations have cron schedule that controls when the operation starts and task definitions have optional monitoring rules with an extra cron expression that allows certain tasks to be scheduled less frequently than the operation level schedule.

**sample operation creation request**
```yaml
Name: tesla_crawl
Type: CRAWL

Policy:
    TaskProfiles:
        - "net_recon"
        - "web_recon"
        - "cloud_recon"

Scope:
    Name: tesla_scope
    ScopeDefinitions:
        - Type: DomainRegex
          Pattern: (^tesla\.com|.*\.tesla\.com)$

Input:
    - tesla.com
```

## How to set it up?

1. create an aws Administrator user & set a local aws profile
2. setup devcontainer
3. put all tool configuration files in the `deployment/` folder
4. run `task deploy`

**To Do**
- [ ] ecs-cli for testing & debugging
- [ ] terraform discord server
