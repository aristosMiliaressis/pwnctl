#!/bin/bash

domain=$1
temp=`mktemp`

python3 /opt/tools/dnsReaper/main.py single --domain $domain --out-format json --out $temp 2>/dev/null

cat $temp | grep -q signature && echo '{"Asset":"'$domain'", "tags":{"ns-takeover":"true"}}'
