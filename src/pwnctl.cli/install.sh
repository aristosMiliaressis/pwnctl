#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

curl https://api.github.com/repos/aristosMiliaressis/pwntainer/releases/latest \
    | jq -r .assets[].browser_download_url \
    | wget -qi -

chmod +x pwnctl
mv pwnctl /usr/bin