#!/bin/bash

url=$1
temp=`mktemp`
RESOLVERS=$(cat /opt/wordlists/dns/resolvers_top25.txt| tr '\n' ',')

echo $url | gau --threads 10 --timeout 25 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | unfurl format %s://%a%p%?%q | tee $temp \
    | xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"gau"}}'

echo $url | waybackurls \
    | unfurl format %s://%a%p%?%q | tee -a $temp \
    | xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"waybackurls"}}'

rm $temp