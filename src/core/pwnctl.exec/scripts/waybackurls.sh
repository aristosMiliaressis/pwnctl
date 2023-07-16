#!/bin/bash

url=$1
temp=`mktemp`
trap "rm $temp" EXIT

waymore -c /opt/tools/waymore/config.yml -mode U -oU $temp -i $(echo $url | unfurl domains) &>/dev/null

cat $temp \
    | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' \
    | urless
