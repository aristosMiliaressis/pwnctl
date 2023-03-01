PWNCTL ![ci: tag](https://github.com/aristosMiliaressis/pwnctl/actions/workflows/ci.yml/badge.svg)
==

serverless configuration driven crawler for recon automation.

### Overview

the target scope is onboarded trough the cli or api, than it is recursivly scanned by pushing tasks to the SQS queue that trigger serverless ECS instances to spin up and consume those tasks, collecting assets into a postgreSQL database, sending discord notifications according to `NotificationRules` and queueing further tasks according to `TaskDefinitions`, all infrastructure is provisioned as code trough CDK.

<p align="center">
  <img src="https://github.com/aristosMiliaressis/pwnctl/blob/master/img/arch-phase1.png?raw=true">
</p>

## `$ pwnctl process`

1. reads from stdin line by line
2. classifies lines into asset classes (NetworkRange/NetworkHost/DomainName/NetworkSocket/DomainNameRecord/HttpEndpoint/HttpParameter)
3. checks if it exists in db
4. if not adds it
5. checks if it is in scope according to `ScopeDefinitions`
6. if not in scope continues to next asset
7. checks `NotificationRules` and sends notifications if any apply
8. checks `TaskDefinitions` for the asset class and queue them according to Task Filter & the target policy
- [x] JSONL(ine) input format with arbitary key/value `Tags` for storing metadata about assets

### In Scope checking rules

- DomainNames are in scope if there is a matching DomainRegex type ScopeDefinition
- NetworkHosts are in scope if there is a matching CIDR ScopeDefinition
- NetworkHosts are in scope if an A record is found linking them to an in scope domain
- HttpEndpoints are in scope if there is a matching UrlRegex type ScopeDefinition
- DomainNameRecords/NetworkSockets/HttpEndpoints/HttpParameters are in scope if they are related to an in scope domain or host

### Scope Configuration

scope definitions can be seeded trough `target-*.yml` files or trough the api/cli.

target policies allow filtering tasks per target.

**`target-*.yml`**
```YAML
Name: EXAMPLE BB TARGET
Platform: HackerOne
Policy:
    Whitelist: ffuf_common
    Blacklist: nmap_basic
    MaxAggressiveness: 6
    AllowActive: true

Scope: 
    - Type: DomainRegex
      Pattern: (^tesla\.com$|.*\.tesla\.com$)
    
    - Type: UrlRegex
      Pattern: (.*:\/\/tsl\.com\/app\/.*$)
    
    - Type: CIDR
      Pattern: 172.16.17.0/24
```

### Task Configuration

- tasks are configured per asset class and can be filtered trough C# script in the `Filter` field. 
- task stdout is read and processed leading to tasks being queued causing a recursive loop.

### Asset Tagging

tags are a way to store arbitary metadata relating to an asset, they can be used in the `Filter` field (trough an indexer on the asset base class) to chain tasks into workflows where one task (e.g nmap) discovers some metadata relating to an asset (e.g. IIS service banner) which than causes a metadata specific task to be queued (e.g. IIS shortname scanning)

**`task-definitions.yml`**
```YAML
- ShortName: ping_sweep
  CommandTemplate: ping-sweep.sh {{CIDR}}
  Subject: NetworkRange

- ShortName: reverse_range_lookup
  CommandTemplate: reverse-range-lookup.sh {{CIDR}}
  Subject: NetworkRange

- ShortName: domain_resolution
  CommandTemplate: dig +short {{Name}} | awk '{print "{{Name}} IN A " $1}'
  Subject: DomainName

- ShortName: httpx
  CommandTemplate: echo {{Name}} | httpx -silent
  Subject: DomainName

- ShortName: zone_trasfer
  CommandTemplate: zone-transfer.sh {{Name}}
  Subject: DomainName

- ShortName: sub_enum
  CommandTemplate: sub-enum.sh {{Name}}
  Filter: DomainName.ZoneDepth <= 2
  Subject: DomainName

- ShortName: asn_lookup
  CommandTemplate: asn-lookup.sh {{IP}}
  Subject: NetworkHost

- ShortName: reverse_lookup
  CommandTemplate: dig +short -x {{IP}}
  Subject: NetworkHost

- ShortName: tcp_scan
  CommandTemplate: tcp-scan.sh {{IP}}
  Subject: NetworkHost

- ShortName: udp_scan
  CommandTemplate: udp-scan.sh {{IP}}
  Subject: NetworkHost

- ShortName: tls_probe
  CommandTemplate: tls-probe.sh {{Address}}
  Subject: NetworkSocket

- ShortName: get_all_urls
  CommandTemplate: get-all-urls.sh {{Url}}
  Subject: HttpEndpoint
  Filter: HttpEndpoint.Path == "/"

- ShortName: dir_brute_common
  CommandTemplate: dir-brute.sh {{Url}} /opt/wordlists/common.txt
  Filter: HttpEndpoint.Path == "/"
  Subject: HttpEndpoint

- ShortName: file_brute_config
  CommandTemplate: file-brute.sh {{Url}} /opt/wordlists/config.txt
  Filter: HttpEndpoint.Path == "/"
  Subject: HttpEndpoint

- ShortName: webcrawl
  CommandTemplate: webcrawl.sh '{{Url}}'
  Filter: HttpEndpoint.Path == "/" || Tags["Content-Type"].Contains("/html") || Tags["Content-Type"].Contains("/xhtml")
  Subject: HttpEndpoint
```

## Notification Configuration

1. worker instances send status notification at start up & shutdown
2. cronjob sends report detailing findings by asset class & task status (pending/completed/failed)
3. configurable notification rules with CSharpScript `Filter` field like `TaskDefinitions` 

**`notification-rules.yml`**
```YAML
- ShortName: default_creds
  Subject: NetworkSocket
  Filter: Tags["vuln-default-creds"] == "true"
  Topic: misconfigs
  
- ShortName: cors_misconfig
  Subject: HttpEndpoint
  Filter: Tags["cors-misconfig"] == "true"
  Topic: misconfigs

- ShortName: shortname_misconfig
  Subject: HttpEndpoint
  Filter: Tags["shortname-misconfig"] == "true"
  Topic: misconfigs

- ShortName: node_debug
  Subject: NetworkSocket
  Filter: NetworkSocket.Port == 9228 || NetworkSocket.Port == 9229
  Topic: misconfigs

- ShortName: docker_api
  Subject: NetworkSocket
  Filter: NetworkSocket.Port == 2735 || NetworkSocket.Port == 2736
  Topic: misconfigs
```

## `$ pwnctl query`

reads sql queries from stdin, executes them and prints the output in JSONL(ine) format

## `$ pwnctl list --class <domains/hosts/endpoints/etc>`

lists assets of the specified class in JSONL(ine) format

## `$ pwnctl export --path out/`

exports all assets in JSONL(ine) format at the specified directory

**To DO**
- [ ]  `$ pwnctl export-db --path dump.sql`

## `$ pwnctl summary`

prints a summary about queued tasks and found assets

## Setup

1. create an iam user with the policy provided in `src/core/pwnctl.infra.cdk/pwnctl-cdk-policy.json`
2. put all configuration/seed/script files in the `deployment/` folder
3. install `task`
> sudo sh -c "$(curl --location https://taskfile.dev/install.sh)" -- -d -b /usr/local/bin
4. run `task simple-setup`

**To Do**
- [ ] terraform discord server
- [ ] private ecr registry
