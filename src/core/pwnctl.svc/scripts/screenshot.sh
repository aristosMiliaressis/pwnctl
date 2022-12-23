#!/bin/bash

url=$1

mkdir -p /mnt/efs/screenshots 2>/dev/null

if [ ! -f "/mnt/efs/screenshots/aquatone_session.json" ] 
then
    echo '{}' > /mnt/efs/screenshots/aquatone_session.json
fi

echo $url \
    | aquatone -silent -chrome-path /usr/bin/chromium -session /mnt/efs/screenshots/aquatone_session.json -http-timeout 30000 -out /mnt/efs/screenshots 2>/dev/null