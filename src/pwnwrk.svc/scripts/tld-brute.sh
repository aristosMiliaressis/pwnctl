#!/bin/bash

temp=`mktemp`; 
domain=$1
RESOLVERS=$(cat /opt/wordlists/dns/resolvers_top25.txt| tr '\n' ',')

dnsrecon -d $domain -t tld -j $temp -n $RESOLVERS >/dev/null 

cat $temp | jq -c .[] | grep "\[{" | jq -r .[].name 

rm $temp