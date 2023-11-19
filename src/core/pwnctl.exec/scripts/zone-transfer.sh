#!/bin/bash

domain=$1
temp=`mktemp`

transferSuccedded=false

nameservers=$(dig +short $domain NS)
for srv in ${nameservers}; do 
    dig +nottlid @$srv $domain AXFR | tee -a $temp | grep -q 'XFR size'
    [ $? -eq 0 ] && transferSuccedded=true
done

cat $temp | sort -u | tr '\t' ' ' | grep ' IN '

if [ $transferSuccedded = true ]; then echo '{"Asset":"'$domain'", "Tags":{"zone_transfer": "true"}}'; fi