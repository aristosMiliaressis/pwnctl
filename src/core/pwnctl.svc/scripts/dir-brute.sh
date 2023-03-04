#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -s -ac -se -o $temp -of json -r -recursion -H "User-Agent: $(uagen)" \
    -mc 200,204,206,300,301,302,303,307,308,400,401,403,405,408,500,501,502 -w $dict -u "${url}FUZZ" &> /dev/null

cat $temp \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's/\//\\\//g')/g"

rm $temp