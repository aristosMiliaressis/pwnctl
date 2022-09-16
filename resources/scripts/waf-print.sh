#!/bin/bash

url=$1
temp="`mktemp`.json"

wafw00f -H "User-Agent: $(uagen)" -o $temp $url > /dev/null 2>&1;

cat $temp | jq .[].firewall | xargs -I _ printf "{\"asset\":\"$url\",\"tags\":{\"waf\":\"_\"}}\n" | grep -v -a "waf:None"

rm $temp