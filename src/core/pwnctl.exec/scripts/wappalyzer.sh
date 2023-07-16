#!/bin/bash
set -euo pipefail 

url=$1
temp=`mktemp`
trap "rm $temp" EXIT

wappy -q -wf $temp -u $url >/dev/null

tags=$(cat $temp \
    | tail -n +2 | cut -d , -f 3- | tr ',' '\t' \
    | awk -F '\t' '{print "\""$1"\":\""$2"\",\""$1" version\":\""$3"\""}' \
    | tr ',' '\n' | grep -v unknown | tr '\n' , | head -c -1)

echo '{"Asset":"'$url'","Tags":{'$tags'}}'
