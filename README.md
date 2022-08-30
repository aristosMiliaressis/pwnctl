
# Architecture

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
- [x] bash job queue

## In Scope checking process

- [x] select all ScopeDefinitions from all programs in db
- [x] iterate over definitions and call bool Matches(ScopeDefinition def) on asset object.
- [x] if any returns true asset is in scope.

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
            "Pattern": "^tesla\\.com$"
        },
        {
            "Type": 0,
            "Pattern": ".*\\.tesla\\.com$"
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

this should probably be in yaml for less character escaping in the command templates, but it will do for now.

**`task-definitions.json`**
```JSON
[
    {
        "ShortName": "ping_sweep",
        "CommandTemplate": "rand=`mktemp`; nmap -sn -PE {{CIDR}} -oG $rand; cat $rand | grep 'Status: Up' | cut -f 2 -d ' ' | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 1,
        "Subject": "NetRange"
    },
    {
        "ShortName": "domain_resolution",
        "CommandTemplate": "dig +short {{Name}} | awk '{print \"{{Name}} IN A \" $1}' | pwnctl process",
        "IsActive": false,
        "Aggressiveness": 1,
        "Subject": "Domain"
    },
    {
        "ShortName": "subfinder",
        "CommandTemplate": "subfinder -silent -d {{Name}} | pwnctl process",
        "IsActive": false,
        "Aggressiveness": 0,
        "Filter": "(Domain.IsRegistrationDomain == true)",
        "Subject": "Domain"
    },
    {
        "ShortName": "reverse_lookup",
        "CommandTemplate": "dig +short -x {{IP}} | pwnctl process",
        "IsActive": false,
        "Aggressiveness": 1,
        "Subject": "Host"
    },
    {
        "ShortName": "nmap_host",
        "CommandTemplate": "nmapTagOutput.sh {{IP}} | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 15,
        "Subject": "Host"
    },
    {
        "ShortName": "get_alt_names",
        "CommandTemplate": "get-alt-names {{IP}} | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 3,
        "Subject": "Host"
    },
    {
        "ShortName": "get_all_urls",
        "CommandTemplate": "echo {{Origin}} | gau --fc 404 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf | unfurl format %s://%a%p%?%q | sort -u | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 10,
        "Subject": "Service",
        "Filter": "(Service[\"Protocol\"] == \"http\") || (Service[\"Protocol\"] == \"https\")"
    },
    {
        "ShortName": "ffuf_common",
        "CommandTemplate": "dir-brute.sh {{Uri}} /tools/wordlist/SecLists/Discovery/Web-Content/common.txt | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 8,
        "Filter": "(Endpoint.Path == \"/\")",
        "Subject": "Endpoint"
    },
    {
        "ShortName": "ffuf_config",
        "CommandTemplate": "dir-brute.sh {{Uri}} /opt/pwntainer/seed/config.txt | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 8,
        "Filter": "(Endpoint.Path == \"/\")",
        "Subject": "Endpoint"
    },
    {
        "ShortName": "hakrawler",
        "CommandTemplate": "echo '{{Uri}}' | hakrawler -insecure -all -plain -headers 'User-Agent: Mozilla/5.0' | pwnctl process",
        "IsActive": true,
        "Aggressiveness": 3,
        "Subject": "Endpoint"
    }
]
```

## `pwnctl --query`

- [x] read sql queries from stdin, execute them and print output
- [x] json output format
- [ ] cli flags for common query types

## `pwnctl -i/--import <importer> -s/--source <source>`

- [ ] burp suite importer

## Workers & Scaling

- [ ] EC2 C&C VM 
- [ ] PostgreSQL db
- [ ] AWS SQS queue 
- [ ] EKS with Fargate for serverless autoscaling worker deployment
- [ ] job runner that will run command and pipe output to `pwnctl` than record task metadata (i.e return code, start/finish timestamp) to db
- [ ] deployment manifest to configure scope/task definitions and scaling
- [ ] slimer worker images for faster worker creation&teardown

## Miscellaneous Stuff

- [ ] non deterministic methods of inscope detection for cloud assets, CIDRs, etc (favicon, legal text, keyword, CT time correlation)
- [ ] maintaining fresh DNS resolvers (scheduled dnsvalidator task will store fresh resolvers for all workers to use)
- [ ] ip/hostname/url normalization
- [ ] IP blocking detection & recycling of cloud ips
- [ ] configurable notifications rules like TaskDefinitions
- [ ] WebUI, graphs and stuff
