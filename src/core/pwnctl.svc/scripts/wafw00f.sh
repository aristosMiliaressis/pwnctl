#!/bin/bash

url=$1
temp="`mktemp`.json"

wafw00f -o $temp $url &> /dev/null

cat $temp | jq .[].firewall | xargs -I _ echo '{"asset":"'$url'","tags":{"waf":"_"}}' | grep -v -a "waf:None" 2>/dev/null

rm $temp 2>/dev/null