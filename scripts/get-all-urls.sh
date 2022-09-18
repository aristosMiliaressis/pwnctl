#!/bin/bash

url=$1

echo $url \
    | gau --fc 404 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | unfurl format %s://%a%p%?%q \
    | sort -u \
    | urgo -H "User-Agent: $(uagen)" -sC -rH 'Content-Type' -rH 'Location' 2>/dev/null \
    | jq '.ResponseHeasers += {"StatusCode": .StatusCode}' \
    | jq -c '{asset:.Url, tags:.ResponseHeasers}'