
# Pwntainer

![ci: tag](https://github.com/aristosMiliaressis/pwntainer/actions/workflows/ci.yml/badge.svg)

 a recursive rule based asset discovery, scanning and reporting framework 

### Implementation Phase #0

- all in one docker container with bash job queue and sqlite db

![arch-phase0](img/arch-phase0.png)

### Implementation Phase #1

- decentralized AWS architecture with autoscaling worker instances and centralized data collection
- hopefully will be as easy as swapping out a `BashJobQueueService` class for a `SQSJobQueueService` a sqlite conn str for postgre and a bit of orchestration shenanigans :)

# `pwnctl --process`

- [x] reads assets from stdin
- [x] classifies assets into classes (Endpoint/Host/Domain/Service/DNSRecord)
- [x] check if exist in db
- [x] if yes do nothing
- [x] if no check if in scope
- [x] if not in scope do nothing
- [x] if inscope query configured task definitions for asset class and queue tasks
- [x] arbitary key/value metadata `Tags`
- [x] bash job queue

## In Scope checking process

- [x] select all ScopeDefinitions from all programs in db
- [x] iterate over definitions and call bool Matches(ScopeDefinition def) on asset object.
- [x] if any returns true asset is in scope.

**To Do**
- out of scope flag on ScopeDefinition

### Scope Configuration

scope definitions are configured in `target-*.json` files along with some policy rules effecting which tasks are allowed to be run against the given scope.

**`target-*.json`**
```JSON
{
    "Name": "EXAMPLE BB TARGET",
    "Platform": "HackerOne",
    "Policy": {
        "Whitelist": "ffuf_common",
        "Blacklist": "nmap_basic",
        "MaxAggressiveness": 6,
        "AllowActive": true
    },
    "Scope": [
        {
	          // DomainRegex = 0, UrlRegex = 1, CIDR = 2
            "Type": 0,
            "Pattern": "(^tesla\\.com$|.*\\.tesla\\.com$)"
        },
        {
            "Type": 2,
            "Pattern": "172.16.17.0/24"
        }
    ]
}
```

## Task Configuration

tasks are configured per asset class and can be filtered trough C# script in the `Filter` field. 

### Asset Tagging

tags are a way to store arbitary metadata relating to an asset, they can be used in the `Filter` field to chain tasks into workflows where one task (e.g nmap) discovers some metadata relating to an asset (e.g. http protocol running on port) which than causes a metadata specific task to be queued (e.g. some http specific task)

**`task-definitions.yml`**
```YAML
- ShortName: ping_sweep
  CommandTemplate: "rand=`mktemp`; nmap -sn {{CIDR}} -oG $rand; cat $rand | grep 'Status: Up' | cut -f 2 -d ' '"
  IsActive: true
  Aggressiveness: 1
  Subject: NetRange

- ShortName: domain_resolution
  CommandTemplate: dig +short {{Name}} | awk '{print "{{Name}} IN A " $1}'
  IsActive: false
  Aggressiveness: 1
  Subject: Domain

- ShortName: sub_enum
  CommandTemplate: sub-enum.sh {{Name}}
  IsActive: false
  Aggressiveness: 0
  Filter: Domain.IsRegistrationDomain == true
  Subject: Domain

- ShortName: tcp_scan
  CommandTemplate: tcp-scan.sh {{IP}}
  IsActive: true
  Aggressiveness: 15
  Subject: Host

- ShortName: dir_brute_common
  CommandTemplate: dir-brute.sh {{Url}} /opt/wordlists/Discovery/Web-Content/common.txt
  IsActive: true
  Aggressiveness: 8
  Filter: Endpoint.Path == "/"
  Subject: Endpoint

- ShortName: hakrawler
  CommandTemplate: "echo '{{Url}}' | hakrawler -insecure -h 'User-Agent: Mozilla/5.0'"
  IsActive: true
  Aggressiveness: 3
  Filter: Endpoint["Content-Type"].Contains("/html") || Endpoint["Content-Type"].Contains("/xhtml")
  Subject: Endpoint
```

## Notification Configuration

...

**`notification-rules.yml`**
```YAML
```

## `pwnctl --query`

- [x] read sql queries from stdin, execute them and print output
- [x] json output format

## `pwnctl list --mode <domains/hosts/endpoints/etc>`

...

## `pwnctl export --path out/`

...

## `pwnctl summary`

...

## `pwnctl -i/--import <importer> -s/--source <source>`

**To Do**
- burp suite importer

## Workers & Scaling

**To Do**
- PostgreSQL db
- AWS SQS queue 
- job runner that will run command and pipe output to `pwnctl` than record task metadata (i.e return code, start/finish timestamp) to db
- EKS with Fargate for serverless autoscaling worker deployment
- slimer worker images for faster worker creation&teardown

## Miscellaneous Stuff

**To Do**
- non deterministic methods of inscope detection for cloud assets, CIDRs, etc (favicon, legal text, keyword, CT time correlation)
- ip/hostname/url normalization
- IP blocking detection & recycling of cloud ips
- WebUI, graphs and stuff
