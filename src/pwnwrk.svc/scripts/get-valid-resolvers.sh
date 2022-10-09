#!/bin/bash

temp=`mktemp`

curl https://public-dns.info/nameservers.txt | shuf -n 200 >$temp

dnsvalidator -tL $temp -threads 10 -o /opt/tools/dnsvalidator/resolvers.txt

head -25 /opt/tools/dnsvalidator/resolvers.txt | tail -n +2 > /opt/wordlists/dns/resolvers_top25.txt

rm /opt/tools/dnsvalidator/resolvers.txt
rm $temp