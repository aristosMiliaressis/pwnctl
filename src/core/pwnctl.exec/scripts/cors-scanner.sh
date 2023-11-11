#!/bin/bash

url=$1

cors-scanner -u $url \
    | jq -c '{asset:"'$url'", tags:{"cors-'$(echo $line | md5sum | cut -d ' ' -f1 | tr -d '\n')'":(.|tostring)}}'
