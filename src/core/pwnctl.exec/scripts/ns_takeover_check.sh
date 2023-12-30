#!/bin/bash
set -eu 

domain=$1
temp=`mktemp`
trap "rm $temp" EXIT

python3 /opt/tools/dnsReaper/main.py single --nocolour --domain $domain --out-format json --out $temp 2>/dev/null

cat $temp | grep -q signature && echo '{"Asset":"'$domain'", "tags":{"ns-takeover":"true"}}' || exit 0