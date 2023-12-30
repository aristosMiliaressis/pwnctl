#!/bin/bash
set -eu

url=$1

headers=(
    'Server'
    'X-Powered-By'
    'Set-Cookie'
    'Location'
    'Content-Security-Policy'
    'X-Frame-Options'
    'Referrer-Policy'
    'Via'
    'X-Served-By'
    'X-Served-From'
    'Server-Timing'
    'X-Cache'
    'X-Cached'
    'X-Cache-Info'
)

params=$(for header in ${headers[@]}; do echo -n "-rH $header "; done)

echo $url \
    | urgo -H "User-Agent: $(uagen)" -title -favicon -sC ${params[@]} \
    | jq '.ResponseHeasers += {"Title": .Title, "Status": .StatusCode|tostring, "FaviconHash": .FaviconHash}' \
    | jq -c '{asset:.Url, tags:.ResponseHeasers}'