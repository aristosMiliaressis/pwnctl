#!/bin/bash

domain=$1
temp=`mktemp`

dnsrecon -t axfr -d $domain > $temp

extract_records() {
    echo "{\"Asset\":\"$domain\", \"Tags\":{\"zone_transfer\":\"true\"}}"
    temp2=`mktemp`

    cat $temp | grep -P '\[\*\] \t' | cut -f2- | xargs -I _ echo "$domain IN _" > $temp2

    cat $temp2 | grep "IN CNAME" | awk '{print $1, $2, $3, $4}' | sort -u
    cat $temp2 | grep "IN NS " | awk '{print $1, $2, $3, $4}' | sort -u
    cat $temp2 | grep "IN TXT "
    cat $temp2 | grep "IN MX "
    cat $temp2 | grep "IN SRV "
    cat $temp2 | grep "IN A " | awk '{print $4, "IN A", $5}' | sed 's/@.//g'
    cat $temp2 | grep "IN AAAA " | awk '{print $4, "IN A", $5}'
    cat $temp2 | grep "IN NSEC" | cut -d ' ' -f 4 | xargs -I _ echo _.$domain | sed 's/@.//g'
    
    rm $temp2
}

cat $temp | grep -q 'Zone Transfer was successful' && extract_records

rm $temp