#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -s -ac -se -r -o $temp -of json -mc 200,206,401 -H "User-Agent: $(uagen)" -w $dict -u ${url}FUZZ &>/dev/null

cat $temp \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's/\//\\\//g')/g"

rm $temp