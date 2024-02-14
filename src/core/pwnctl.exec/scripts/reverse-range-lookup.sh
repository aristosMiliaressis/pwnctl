#!/bin/bash
set -eu

cidr=$1
temp=`mktemp`

mapcidr -silent -cidr $cidr | hakip2host > $temp

cat $temp | grep -E '^\[SSL-' | awk '{print $2 ":443\t" $3}' | sed 's/*\.//' | sort -u

cat $temp | grep -E '^\[DNS-' | while read line; do \
        ip=$(echo $line | cut -d ' ' -f2 | awk -F. '{print $4"."$3"." $2"."$1".in-addr.arpa"}'); \
        domain=$(echo $line | cut -d ' ' -f3); \
        echo "$ip IN PTR $domain"; \
done