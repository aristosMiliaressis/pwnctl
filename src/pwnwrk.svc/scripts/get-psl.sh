#!/bin/bash

mkdir "/opt/wordlists/dns/" 2>/dev/null

wget https://publicsuffix.org/list/public_suffix_list.dat -P "/opt/wordlists/dns/"

cat "/opt/wordlists/dns/public_suffix_list.dat" | grep -v "//" | sed '/^[[:space:]]*$/d' > tmp.dat
cat tmp.dat > "/opt/wordlists/dns/public_suffix_list.dat"
rm tmp.dat