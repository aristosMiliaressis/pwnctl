#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -s -se -ac -acp -acs extra -ar -recursion -recursion-status 301,302,303,307,308,401,403 -o $temp -of json -H "User-Agent: $(uagen)" -w $dict -u ${url}FUZZ &>/dev/null

cat $temp \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's/\//\\\//g')/g"

rm $temp