#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

curl https://api.github.com/repos/aristosMiliaressis/pwntainer/releases/latest \
    | jq -r .assets[].browser_download_url \
    | wget -qi -

tar -xzf pwnctl-linux64.tar.gz -C /opt/pwnctl
rm pwnctl-linux64.tar.gz

chmod +x /opt/pwnctl/pwnctl
ln -s /opt/pwnctl/pwnctl /usr/loca/bin/pwnctl
