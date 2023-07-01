#!/bin/bash

url=$1
temp="`mktemp`.json"

wafw00f -o $temp $url &> /dev/null
waf=$(cat $temp | jq .[].firewall | sed 's/"None"/null/')

cdncheck -silent -j -resp -i pentesterlab.com | jq -c "{asset:\"$url\", tags:{cloud:.cloud_name,cdn:.cdn_name,waf:$waf}}"

rm $temp 2>/dev/null