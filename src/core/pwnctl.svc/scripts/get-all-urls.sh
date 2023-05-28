#!/bin/bash

domain=$1

temp=`mktemp`
trap "rm $temp" EXIT

echo $domain \
    | gau --subs --threads 40 --timeout 20 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf > $temp

echo $url | waybackurls \
    | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' >> $temp

cat $temp | urless
