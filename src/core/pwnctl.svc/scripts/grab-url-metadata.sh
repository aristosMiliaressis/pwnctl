#!/bin/bash

url=$1

echo $url \
    | urgo -H "User-Agent: $(uagen)" -title -favicon -sC -rH Server -rH X-Powered-By -rH Set-Cookie -rH Via -rH Content-Type -rH Location -rH Content-Security-Policy \
    | jq '.ResponseHeasers += {"Title": .Title, "Status": .StatusCode, "FaviconHash": .FaviconHash}' \
    | jq -c '{asset:.Url, tags:.ResponseHeasers}'
 