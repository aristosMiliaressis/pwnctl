#!/bin/bash

url=$1

echo $url | waybackurls \
    | grep . \
    | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' \
    | unfurl format '%s://%a%p%?%q' \
    | sort -u
