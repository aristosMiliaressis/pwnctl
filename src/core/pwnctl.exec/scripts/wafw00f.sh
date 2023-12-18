#!/bin/bash
set -x

url=$1
domain=$(echo $url | unfurl domains)

waf=$(wafw00f -a $url | grep '[+].*is behind' | cut -d ' ' -f 7- | rev | cut -d ' ' -f 2- | rev | sed 's/"None"//')
[ -z "$waf" ] && waf=""

cdncheck -silent -j -resp -i $domain | jq -c "{asset:\"$url\", tags:{cloud:.cloud_name,cdn:.cdn_name,waf:\"$waf\"}}"
