#!/bin/bash

url=$1
dict=$2
temp=`mktemp`; 


ffuf -o $temp -of json  -H 'User-Agent: Mozilla/5.0' -H 'X-Forwarded-For: 127.0.0.1' -w $dict -u $urlFUZZ

cat $temp | jq  -r .results[].input.FUZZ | cut -c2- | while read path; do echo "$url${path:1}"; done
rm $temp