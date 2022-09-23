#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

curl https://api.github.com/repos/aristosMiliaressis/pwntainer/releases/latest 2>/dev/null \
    | jq -r .assets[].browser_download_url \
    | wget -qi -

tar -xzf pwnctl-linux64.tar.gz -C /opt/
rm pwnctl-linux64.tar.gz

chmod +x /opt/pwnctl/pwnctl
ln -s /opt/pwnctl/pwnctl /usr/local/bin/pwnctl 2>/dev/null

mkdir /etc/pwnctl 2>/dev/null
mv /opt/pwnctl/config.ini /etc/pwnctl

echo "All done!"
echo "Remember to setup your connection string in /etc/pwnctl/config.ini"
