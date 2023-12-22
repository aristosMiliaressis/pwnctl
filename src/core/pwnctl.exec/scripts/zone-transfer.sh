#!/bin/bash
set -eux

domain=$1
temp=`mktemp`
trap "rm $temp" EXIT

transferSuccedded=false

nameservers=$(dig +short $domain NS)
for srv in ${nameservers}; do 
    dig +nottlid @$srv $domain AXFR | tee -a $temp | grep -q 'XFR size' || echo -n
    [ $? -eq 0 ] && transferSuccedded=true
done

cat $temp | sort -u | tr '\t' ' ' | grep ' IN ' || echo -n

if [ $transferSuccedded = true ]; then echo '{"Asset":"'$domain'", "Tags":{"zone_transfer": "true"}}'; fi