#!/bin/bash

domain=$1

resp=$(dig +nottlid $domain)

echo "$resp" | tr '\t' ' ' | grep ' IN CNAME '
echo "$resp" | tr '\t' ' ' | grep ' IN A '
echo "$resp" | tr '\t' ' ' | grep ' IN AAAA '

expr='status: (NXDOMAIN|SERVFAIL)'
[[ $resp =~ $expr ]] && echo "{\"Asset\":\"$domain\",\"Tags\":{\"rcode\":\"${BASH_REMATCH[1]}\"}}"