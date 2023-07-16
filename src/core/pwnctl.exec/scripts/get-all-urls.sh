#!/bin/bash

domain=$1

temp=`mktemp`
trap "rm $temp" EXIT

waymore -lcc 1 -c /opt/tools/waymore/config.yml -mode U -oU $temp -i $domain &>/dev/null

echo $domain \
    | gau --subs --threads 40 --timeout 20 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf >> $temp

cat $temp | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' | urless
