#!/bin/bash
set -eu

domain=$1
temp=`mktemp`
trap "rm $temp" EXIT

nameservers=$(dig +short $domain NS)
for srv in ${nameservers}; do 
    transferSuccedded=true
    dig +nottlid @$srv $domain AXFR | tee -a $temp | grep -q 'XFR size' || transferSuccedded=false
    if [ $transferSuccedded = true ]; 
    then 
        cat $temp | sort -u | tr '\t' ' ' | grep ' IN ' || echo -n
        echo '{"Asset":"'$domain'", "Tags":{"zone_transfer": "true"}}'
        exit 0
    fi
done
