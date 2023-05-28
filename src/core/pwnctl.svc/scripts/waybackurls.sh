#!/bin/bash

url=$1

echo $url | waybackurls \
    | grep -vE '\.(jpg|png|svg|jpeg|gif|ico|ttf|woff|woff2|eot|css|pdf)\b' \
    | urless
