PWNCTL ![ci: tag](https://github.com/aristosMiliaressis/pwnctl/actions/workflows/ci.yml/badge.svg)
==

serverless configuration driven crawler for recon automation.

### Overview

the target scope is onboarded trough the cli or api, than it is recursivly scanned by pushing tasks to the SQS queue that trigger serverless ECS instances to spin up and consume those tasks, collecting assets into an Aurora postgreSQL database, sending discord notifications according to `NotificationRules` and queueing further tasks according to `TaskDefinitions`, all infrastructure is provisioned as code trough CDK.

<p align="center">
  <img src="https://github.com/aristosMiliaressis/pwnctl/blob/master/img/arch-phase1.png?raw=true">
</p>

## `$ pwnctl process`

1. reads from stdin line by line
2. classifies lines into asset classes (NetRange/Host/Domain/Service/DNSRecord/Endpoint/Parameter)
3. checks if it exists in db
4. if not adds it
5. checks if it is in scope according to `ScopeDefinitions`
6. if not in scope continues to next asset
7. checks `NotificationRules` and sends notifications if any apply
8. checks `TaskDefinitions` for the asset class and queue them according to Task Filter & the target policy
- [x] JSONL(ine) input format with arbitary key/value `Tags` for storing metadata about assets

### In Scope checking rules

- Domains are in scope if there is a matching DomainRegex type ScopeDefinition
- Hosts are in scope if there is a matching CIDR ScopeDefinition
- Hosts are in scope if an A record is found linking them to an in scope domain
- Endpoints are in scope if there is a matching UrlRegex type ScopeDefinition
- DNSRecords/Services/Endpoints/Parameters are in scope if they are related to an in scope domain or host

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
  Subject: NetRange

- ShortName: reverse_range_lookup
  CommandTemplate: reverse-range-lookup.sh {{CIDR}}
  Subject: NetRange

- ShortName: domain_resolution
  CommandTemplate: dig +short {{Name}} | awk '{print "{{Name}} IN A " $1}'
  Subject: Domain

- ShortName: httpx
  CommandTemplate: echo {{Name}} | httpx -silent
  Subject: Domain

- ShortName: zone_trasfer
  CommandTemplate: zone-transfer.sh {{Name}}
  Subject: Domain

- ShortName: sub_enum
  CommandTemplate: sub-enum.sh {{Name}}
  Filter: Domain.ZoneDepth <= 2
  Subject: Domain

- ShortName: tld_brute
  CommandTemplate: tld-brute.sh {{Name}}
  Subject: Domain
  Filter: Domain.ZoneDepth == 1

- ShortName: asn_lookup
  CommandTemplate: asn-lookup.sh {{IP}}
  Subject: Host

- ShortName: reverse_lookup
  CommandTemplate: dig +short -x {{IP}}
  Subject: Host

- ShortName: tcp_scan
  CommandTemplate: tcp-scan.sh {{IP}}
  Subject: Host

- ShortName: udp_scan
  CommandTemplate: udp-scan.sh {{IP}}
  Subject: Host

- ShortName: get_alt_names
  CommandTemplate: get-alt-names.sh {{IP}}
  Subject: Host

- ShortName: get_all_urls
  CommandTemplate: get-all-urls.sh {{Url}}
  Subject: Endpoint
  Filter: Endpoint.Path == "/"

- ShortName: dir_brute_common
  CommandTemplate: dir-brute.sh {{Url}} /opt/wordlists/common.txt
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: file_brute_config
  CommandTemplate: file-brute.sh {{Url}} /opt/wordlists/config.txt
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: wafw00f
  CommandTemplate: wafw00f.sh {{Url}}
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: grab_url_metadata
  CommandTemplate: grab-url-metadata.sh {{Url}}
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: webanalyze
  CommandTemplate: webanalyze.sh {{Url}}
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: screenshot
  CommandTemplate: screenshot.sh {{Url}}
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: webcrawl
  CommandTemplate: webcrawl.sh '{{Url}}'
  Filter: Endpoint.Path == "/" || Tags["Content-Type"].Contains("/html") || Tags["Content-Type"].Contains("/xhtml")
  Subject: Endpoint

- ShortName: link_finder
  CommandTemplate: link-finder.sh {{Url}}
  Filter: Endpoint.Extension == "js" || Endpoint.Extension == "jsm" || Tags["Content-Type"].Contains("/javascript")
  Subject: Endpoint

- ShortName: cors_check
  CommandTemplate: check-cors.sh {{Url}}
  Subject: Endpoint
  Filter: Endpoint.Path == "/" || Tags["Content-Type"].Contains("/json") || Tags["Content-Type"].Contains("/xml")

- ShortName: shortname_scanning
  CommandTemplate: shortname-check.sh {{Url}}
  Subject: Endpoint
  Filter: Tags["Title"].Contains("Internet Information Services")

- ShortName: shortname_scanning
  CommandTemplate: shortname-check.sh {{Origin}}
  Subject: Service
  Filter: Tags["Version"].Contains("IIS") || Tags["Version"].Contains("Microsoft HTTPAPI")

- ShortName: zone_walking
  CommandTemplate: ldns-walk {{Key}} | tail -n +2 | cut -d ' ' -f 1
  Filter: DNSRecord.Type == pwnctl.domain.Enums.DnsRecordType.NSEC
  Subject: DNSRecord

- ShortName: cloud_enum
  CommandTemplate: cloud-enum.sh {{Word}}
  Subject: Keyword
```

## Notification Configuration

1. worker instances send status notification at start up & shutdown
2. cronjob sends report detailing findings by asset class & task status (pending/completed/failed)
3. configurable notification rules with CSharpScript `Filter` field like `TaskDefinitions` 

**`notification-rules.yml`**
```YAML
- ShortName: default_creds
  Subject: Service
  Filter: Tags["vuln-default-creds"] == "true"
  Topic: misconfigs
  
- ShortName: cors_misconfig
  Subject: Endpoint
  Filter: Tags["cors-misconfig"] == "true"
  Topic: misconfigs

- ShortName: shortname_misconfig
  Subject: Endpoint
  Filter: Tags["shortname-misconfig"] == "true"
  Topic: misconfigs

- ShortName: node_debug
  Subject: Service
  Filter: Service.Port == 9228 || Service.Port == 9229
  Topic: misconfigs

- ShortName: docker_api
  Subject: Service
  Filter: Service.Port == 2735 || Service.Port == 2736
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
