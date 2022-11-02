#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

curl https://api.github.com/repos/aristosMiliaressis/pwnctl/releases/latest 2>/dev/null \
    | grep browser_download_url | awk '{print $2}' | tr -d '"' \
    | wget -qi -

chmod +x pwnctl
mv pwnctl /usr/local/bin

mkdir /etc/pwnctl 2>/dev/null
curl https://raw.githubusercontent.com/aristosMiliaressis/pwnctl/master/src/pwnctl/pwnctl.cli/scripts/config.ini -o /etc/pwnctl/config.ini 

echo "All done!"
