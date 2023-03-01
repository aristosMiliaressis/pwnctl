#!/bin/bash

url=$1

mkdir -p /mnt/efs/screenshots 2>/dev/null

echo $url \
    | aquatone -silent -chrome-path /usr/bin/chromium -http-timeout 30000 -out /mnt/efs/screenshots
