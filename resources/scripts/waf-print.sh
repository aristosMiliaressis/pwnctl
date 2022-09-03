#!/bin/bash

url=$1
temp="`mktemp`.json"

wafw00f -o $temp $url > /dev/null 2>&1;

cat $temp | jq .[].firewall | xargs -I _ echo "$url [[waf:_]]" | grep -v "waf:None"

rm $temp