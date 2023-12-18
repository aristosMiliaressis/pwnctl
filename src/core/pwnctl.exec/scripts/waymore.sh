#!/bin/bash

domain=$1

timeout -k 1m 150m waymore -lcc 1 -c /opt/tools/waymore/config.yml -mode U -i $domain \
    --timeout 15 --processes 4 --limit-requests 40000 2>/dev/null \
    | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' \ 
    | urless