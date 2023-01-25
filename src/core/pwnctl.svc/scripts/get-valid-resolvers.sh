#!/bin/bash

echo "$HOSTNAME - Getting fresh resolvers"

temp=`mktemp`
temp2=`mktemp`

curl -s https://public-dns.info/nameservers.txt | shuf -n 5000 >$temp 2>/dev/null

dnsvalidator -tL $temp -threads 100 -o $temp2 &>/dev/null

head -30 $temp2 | tail -n +2 > /opt/wordlists/dns/resolvers_top25.txt

echo "$(wc -l /opt/wordlists/dns/resolvers_top25.txt) valid resolver(s) found."

rm $temp2
rm $temp