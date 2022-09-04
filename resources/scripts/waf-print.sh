#!/bin/bash

url=$1
temp="`mktemp`.json"

wafw00f -o $temp $url > /dev/null 2>&1;

cat $temp | jq .[].firewall | xargs -I _ printf "$url${PWNCTL_DELIMITER}waf:_" | grep -v -a "waf:None"

rm $temp