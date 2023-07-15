#!/bin/bash

url=$1
domain=$(echo $url | unfurl domains)

waf=$(wafw00f -a --no-colors $url | grep '[+].*is behind' | cut -d ' ' -f 7- | rev | cut -d ' ' -f 2- | rev 2>/dev/null | sed 's/"None"//')
[ -z "$waf" ] && waf=""

cdncheck -silent -j -resp -i $domain | jq -c "{asset:\"$url\", tags:{cloud:.cloud_name,cdn:.cdn_name,waf:\"$waf\"}}"

rm $temp 2>/dev/null
