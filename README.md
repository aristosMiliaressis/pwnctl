PWNCTL ![ci: tag](https://github.com/aristosMiliaressis/pwnctl/actions/workflows/ci.yml/badge.svg)
==

serverless configuration driven framework for recon automation.

</br>

**Table of Contents**
1. [What is this?](#what)
2. [How does it Work?](#how)</br>
  2.1. [Assets & Tags](#assets)</br>
  2.2. [Scope](#scope)</br>
  2.3. [Tasks & Notifications](#tasks)</br>
  2.4. [Operations](#ops)
3. [How to set it up?](#setup)

### [What is this?](#what)

a framework for external recon operations that facilitates discovering, scanning and monitoring assets trough a configurable engine running on autoscaling aws infrastructure.

### [How does it Work?](#how)

<p align="center">
  <img src="https://github.com/aristosMiliaressis/pwnctl/blob/master/img/arch-phase1.png?raw=true">
</p>

tasks are placed in an sqs queue that causes container instances to spin up perform the recon tasks and process the output according to configuration.


#### [Assets & Tags](#assets)

task output is processed line by line, each line is classified into one of the following asset classes.

**asset classification**

| Asset            |   Notation                                       |
|------------------|:------------------------------------------------:|  
| DomainName       | example.com                                      |
| DomainNameRecord | example.com A 1.3.3.7                            |
| Email            | johndoe@example.com                              |
| HttpEndpoint     | https://example.com/api                          |
| HttpHost         | 1.3.3.7:443  example.com                         |
| HttpParameter    | https://example.com/api?token=                   |
| NetworkHost      | 1.3.3.7, FD00:DEAD:BEEF:64:35::2                 |
| NetworkRange     | 12.34.45.0/24, 2001:db8::/48                     |
| NetworkSocket    | 1.3.3.7:443, tcp://1.3.3.7:22, udp://1.3.3.7:161 |

lines can either be in raw or json format, json format is used to assign tags to the asset.

```
example.com
example.com:443
{"Asset":"https://example.com/api","Tags":{"Name":"Server","Value":"IIS"}}
sub.example.com
sub2.example.com
```

**Tags** are a way to store arbitary metadata relating to an asset, they can be used in the `Filter` field (trough an indexer on the asset base class) to chain tasks into workflows where one task (e.g nmap) discovers some metadata relating to an asset (e.g. IIS service banner) which than causes a metadata specific task to be queued (e.g. IIS shortname scanning)

#### [Scope](#scope)

scope is determined trough explicit matching of `ScopeDefinition` patterns or trough a set of relationship rules

scope definitions can be one of three types (DomainRegex, UrlRegex & CIDR), and can be grouped into ScopeAggregates

aside from direct matching assets will be considered inscope according to the following rules
- DomainNameRecords beloinging to  in scope domain are inscope
- Ip addreses connected to inscope domain trough A/AAAA record are also inscope
- Domains connected to inscope domain trough CNAME record are NOT inscope
- NetworkSockets/HttpEndpoints/HttpParameters containing inscope ips or domains are also inscope
- Emails containing inscope domain are in scope

**sample ScopeAggregate creation request**
```json
{
  "ShortName": "tesla_scope",
  "ScopeDefinitions":
  [
    { "Type": 0, "Pattern": "(^tesla\\.com$|.*\\.tesla\\.com$)" },
    { "Type": 1, "Pattern": "(.*:\\/\\/tsl\\.com\\/app\\/.*$)" },
    { "Type": 2, "Pattern": "172.16.17.0/24" }
  ]
}
```

#### [Tasks & Notifications](#tasks)

tasks are configured trough TaskDefinitions and can be organized into TaskProfiles.

tasks definitions are specific to an assset class (i.e domain_resolution task only runs on DomainName assets)

assets can be filtered trough the CSharpScript `Filter` field that has access to the asset class and associated Tags.

**`task-definitions.yml`**
```YAML
Profiles: [ "all" ]
TaskDefinitions:
  - Name: ping_sweep
    CommandTemplate: ping-sweep.sh {{CIDR}}
    Subject: NetworkRange

  - Name: reverse_range_lookup
    CommandTemplate: reverse-range-lookup.sh {{CIDR}}
    Subject: NetworkRange

  - Name: domain_resolution
    CommandTemplate: dig +short {{Name}} | awk '{print "{{Name}} IN A " $1}'
    Subject: DomainName

  - Name: httpx
    CommandTemplate: echo {{Name}} | httpx -silent
    Subject: DomainName

  - Name: zone_trasfer
    CommandTemplate: zone-transfer.sh {{Name}}
    Subject: DomainName

  - Name: sub_enum
    CommandTemplate: sub-enum.sh {{Name}}
    Filter: DomainName.ZoneDepth <= 2
    Subject: DomainName

  - Name: asn_lookup
    CommandTemplate: asn-lookup.sh {{IP}}
    Subject: NetworkHost

  - Name: reverse_lookup
    CommandTemplate: dig +short -x {{IP}}
    Subject: NetworkHost

  - Name: tcp_scan
    CommandTemplate: tcp-scan.sh {{IP}}
    Subject: NetworkHost

  - Name: udp_scan
    CommandTemplate: udp-scan.sh {{IP}}
    Subject: NetworkHost

  - Name: tls_probe
    CommandTemplate: tls-probe.sh {{Address}}
    Subject: NetworkSocket

  - Name: get_all_urls
    CommandTemplate: get-all-urls.sh {{Url}}
    Subject: HttpEndpoint
    Filter: HttpEndpoint.Path == "/"

  - Name: dir_brute_common
    CommandTemplate: dir-brute.sh {{Url}} /opt/wordlists/common.txt
    Filter: HttpEndpoint.Path == "/"
    Subject: HttpEndpoint

  - Name: file_brute_config
    CommandTemplate: file-brute.sh {{Url}} /opt/wordlists/config.txt
    Filter: HttpEndpoint.Path == "/"
    Subject: HttpEndpoint

  - Name: webcrawl
    CommandTemplate: webcrawl.sh '{{Url}}'
    Filter: HttpEndpoint.Path == "/" || Tags["Content-Type"].Contains("/html") || Tags["Content-Type"].Contains("/xhtml")
    Subject: HttpEndpoint
```

**Notification Configuration**

1. worker instances send status notification at start up & shutdown
2. cronjob sends report detailing findings by asset class & task status (pending/completed/failed) [!WorkInProgress!]
3. configurable notification rules with CSharpScript `Filter` field like `TaskDefinitions` 

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

#### [Operations](#ops)

there are three types of operations Crawl, Scan & Monitor.

every operation has a ScopeAggregate and a TaskProfile.

**Crawl** operations have an addition Input list that contains the initial assets that should be processed to start a recursive discovery loop.

**Scan** operations can be used to expand on the collected assets and discover misconfigurations in a controlled manner.

**Monitor** operations allow you to periodicly monitor assets for change.

**To Do**
- [ ] Implement schedule based Monitor operations with EventBridge

### [How to set it up?](#setup)

1. create an aws Administrator user for use with terraform
2. put all configuration/seed/script files in the `deployment/` folder
3. install `task`
> sudo sh -c "$(curl --location https://taskfile.dev/install.sh)" -- -d -b /usr/local/bin
4. run `task simple-setup`

**To Do**
- [ ] terraform private ecr registry
- [ ] terraform discord server
