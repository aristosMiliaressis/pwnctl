#!/bin/bash

temp=`mktemp` 
cidr=$1

dnsrecon -r $cidr -t rvl -j $temp >/dev/null

cat $temp | jq -c .[] | grep "\[{" | jq -r .[].name

rm $temp