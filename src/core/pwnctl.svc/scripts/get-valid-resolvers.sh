#!/bin/bash

echo "Getting fresh resolvers"

temp=`mktemp`
temp2=`mktemp`

curl https://public-dns.info/nameservers.txt | shuf -n 1500 >$temp

dnsvalidator -tL $temp -threads 100 -o $temp2

head -25 $temp2 | tail -n +2 > /opt/wordlists/dns/resolvers_top25.txt

echo "$(wc -l /opt/wordlists/dns/resolvers_top25.txt) valid resolver(s) found."

rm $temp2
rm $temp