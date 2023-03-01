#!/bin/bash

domain=$1

resp=$(dig +nottlid $domain)

echo "$resp" | tr '\t' ' ' | grep -E ' IN (A|AAAA|CNAME) '

expr='status: (NXDOMAIN|SERVFAIL|REFUSED)'
[[ $resp =~ $expr ]] && echo '{"Asset":"'$domain'","Tags":{"rcode":"'${BASH_REMATCH[1]}'"}}'