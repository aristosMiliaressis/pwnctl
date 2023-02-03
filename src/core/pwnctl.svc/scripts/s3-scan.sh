#!/bin/bash

url=$1

if [[ "$url" == *"://s3."* ]]; 
then 
    bucketName=$(echo $url | sed -E 's/http.?:\/\///g' | cut -d '/' -f 2)
else
    bucketName=$(echo $url | sed -E 's/http.?:\/\///g' | sed 's/\.s3\..*//g')
fi

resp=$(curl -L -k -s $url) 
echo $resp | grep -q NoSuchBucket && echo "{\"Asset\":\"$url\",\"Tags\"{\"s3-takeover\":\"true\"}}"
echo $resp | grep -q "The specified bucket does not exist" && echo "{\"Asset\":\"$url\",\"Tags\":{\"s3-takeover\":\"true\"}}"

s3scanner -i scan -b $bucketName \
    | grep bucket_exists \
    | cut -d '|' -f 3 \
    | while read perms; do if [ "${#perms}" -gt 30 ]; then \
            echo "{\"Asset\":\"$url\",\"Tags\"{\"s3-public-perms\":\"$perms\"}}"; \
        fi; done