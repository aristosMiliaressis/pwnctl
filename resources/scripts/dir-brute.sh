#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -o $temp -of json -r -H "User-Agent: $(uagen)" -H 'X-Forwarded-For: 127.0.0.1' -w $dict -u ${url}FUZZ > /dev/null 2>&1

cat $temp \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,"content-type":.["content-type"]}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | tr "/" "\/")/g"

rm $temp