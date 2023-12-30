#!/bin/bash
set -eu

domain=$1

timeout --preserve-status -k 1m 150m /opt/tools/src/waymore/waymore.py -lcc 1 -c /opt/tools/waymore/config.yml -mode U -i $domain \
    --timeout 15 --processes 4 --limit-requests 40000 \
    | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' | urless