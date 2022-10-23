#!/bin/bash

url=$1
temp=`mktemp`

whatweb -U "$(uagen)" --log-brief=$temp $url >/dev/null 

cat $temp \
    | tr ',' '\n' | grep -Eo ' [A-Z].*\[.*\]' | tr -d ']' | tr '[' '\t' | awk '{print "\"" $1 "\":" "\"" $2 "\""}' | tr '\n' ',' | head -c -1 | awk '{print "{" $1 "}" }' | jq -c "{asset:\"$url\", tags:.}"

rm $temp
