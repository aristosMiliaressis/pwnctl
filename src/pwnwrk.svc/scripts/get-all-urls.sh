#!/bin/bash

url=$1
temp=`mktemp`
temp2=`mktemp`

echo $url | gau --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | unfurl format %s://%a%p%?%q > $temp

echo $url | waybackurls \
    | unfurl format %s://%a%p%?%q >> $temp

cat $temp | sort -u \
    | httpx -fc 404 -nc -silent -sc -ct -cl -location -json -o $temp2 2>&1 >/dev/null

cat $temp2 | jq -c '{Asset:.url, tags:{Status:.["status-code"], "Content-Type":.["content-type"], "Content-Length":.["content-length"],location:.location}}'

rm $temp2
rm $temp