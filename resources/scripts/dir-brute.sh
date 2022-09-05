#!/bin/bash

url=$1
dict=$2
temp=`mktemp`

ffuf -o $temp -of json -r -H 'User-Agent: Mozilla/5.0' -H 'X-Forwarded-For: 127.0.0.1' -w $dict -u ${url}FUZZ > /dev/null 2>&1

cat $temp \
    | jq -r  '.results[] | { path: .input.FUZZ, status: .status, ct: .["content-type"] } | "\(.path)%%%%%Status:\(.status)%%%%%Content-Type:\(.ct)"' \
    | sed "s/%%%%%/$PWNCTL_DELIMITER/g" \
    | while read asset; do echo "$url$asset"; done

rm $temp