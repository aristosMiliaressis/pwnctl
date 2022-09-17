#!/bin/bash

temp=`mktemp`

curl https://public-dns.info/nameservers.txt | shuf -n 200 >$temp

dnsvalidator -tL $temp -threads 10 -o /opt/dnsvalidator/resolvers.txt

head -25 /opt/dnsvalidator/resolvers.txt > /opt/wordlists/dns/resolvers_top25.txt

rm /opt/dnsvalidator/resolvers.txt
rm $temp