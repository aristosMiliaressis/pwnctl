#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -ac -se -maxtime 5400 -o $temp -of json -r -recursion -H "User-Agent: $(uagen)" -w $dict -u ${url}FUZZ > /dev/null 2>&1

cat $temp \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's/\//\\\//g')/g"

rm $temp