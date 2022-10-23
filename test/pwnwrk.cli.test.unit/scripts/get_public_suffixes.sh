#!/bin/bash

mkdir "./dns/" 2>/dev/null

wget https://publicsuffix.org/list/public_suffix_list.dat -P "./dns/"

cat "./dns/public_suffix_list.dat" | grep -v "//" | sed '/^[[:space:]]*$/d' > tmp.dat
cat tmp.dat > "./dns/public_suffix_list.dat"
rm tmp.dat
