#!/bin/bash

domain=$1

resp=$(dig +nottlid SOA $domain A $domain AAAA $domain CNAME $domain) 2>/dev/null

echo "$resp" | tr '\t' ' ' | grep -E ' IN (A|AAAA|CNAME|SOA) '

expr='status: (NXDOMAIN|SERVFAIL|REFUSED)'
[[ $resp =~ $expr ]] && echo '{"Asset":"'$domain'","Tags":{"rcode":"'${BASH_REMATCH[1]}'"}}'