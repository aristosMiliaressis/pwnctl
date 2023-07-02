#!/bin/bash

url=$1
domain=$(echo $url | unfurl domains)
temp="`mktemp`.json"

wafw00f -o $temp $url &> /dev/null
waf=$(cat $temp | jq .[].firewall | sed 's/"None"/null/')
[ -z $waf ] && waf="null"

cdncheck -silent -j -resp -i $domain | jq -c "{asset:\"$url\", tags:{cloud:.cloud_name,cdn:.cdn_name,waf:$waf}}"

rm $temp 2>/dev/null
