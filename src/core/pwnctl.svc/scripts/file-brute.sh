#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -t 20 -s -o $temp -of json -fc 200 -mc 200,206,401 -w $dict -u ${url}FUZZ &>/dev/null

cat $temp \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's/\//\\\//g')/g"

rm $temp