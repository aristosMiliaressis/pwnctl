#!/bin/bash

url=$1
temp=`mktemp`

echo $url \
    | gau --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | unfurl format %s://%a%p%?%q \
    | sort -u \
    | httpx -fc 404 -nc -silent -sc -ct -cl -location -json -o $temp 2>&1 >/dev/null

cat $temp | jq -c '{Asset:.url, tags:{StatusCode:.["status-code"], "Content-Type":.["content-type"], "Content-Length":.["content-length"],location:.location}}'

rm $temp