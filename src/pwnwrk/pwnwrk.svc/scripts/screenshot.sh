#!/bin/bash

url=$1

echo $url \
    | aquatone -silent -chrome-path /usr/bin/chromium -http-timeout 30000 -out /mnt/efs/screenshots