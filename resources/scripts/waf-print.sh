#!/bin/bash

url=$1
temp="`mktemp`.json"

wafw00f -o $temp $url > /dev/null 2>&1;

cat $temp | jq .[].firewall | printf "{\"asset\":\"$url\",\"tags\":{\"waf\":\"_\"]}\n" | grep -v -a "waf:None"

rm $temp