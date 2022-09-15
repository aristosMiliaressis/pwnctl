#!/bin/bash

url=$1

echo $url \
    | gau --fc 404 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | unfurl format %s://%a%p%?%q \
    | sort -u \
    | urgo -H 'User-Agent: Mozilla/5.0' -rH 'Content-Type' 2>/dev/null \
    | jq '{asset:.Url, tags:.ResponseHeasers}' 2>/dev/null