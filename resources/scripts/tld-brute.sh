#!/bin/bash

temp=`mktemp`; 
domain=$1

dnsrecon -d $domain -t tld -j $temp >/dev/null 

cat $temp | jq -c .[] | grep "\[{" | jq -r .[].name 

rm $temp