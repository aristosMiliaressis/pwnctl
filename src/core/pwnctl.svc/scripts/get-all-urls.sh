#!/bin/bash

url=$1

echo $url | gau --threads 20 --timeout 25 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | unfurl format '%s://%a%p%?%q' \
    | xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"gau"}}'

echo $url | waybackurls \
    | unfurl format '%s://%a%p%?%q' \
    | xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"waybackurls"}}'
