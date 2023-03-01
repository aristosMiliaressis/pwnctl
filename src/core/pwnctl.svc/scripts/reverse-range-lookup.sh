#!/bin/bash

temp=`mktemp` 
cidr=$1
RESOLVERS=$(cat /opt/wordlists/dns/resolvers.txt| tr '\n' ',')

dnsrecon -r $cidr -t rvl -j $temp -n $RESOLVERS >/dev/null

cat $temp | jq -c .[] | grep "\[{" | jq -r .[].name

rm $temp