#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

curl https://api.github.com/repos/aristosMiliaressis/pwnctl/releases/latest 2>/dev/null \
    | grep browser_download_url | awk '{print $2}' | tr -d '"' \
    | wget -qi -

tar -xzf pwnctl-linux64.tar.gz -C /opt/
rm pwnctl-linux64.tar.gz

chmod +x /opt/pwnctl/pwnctl
ln -s /opt/pwnctl/pwnctl /usr/local/bin/pwnctl 2>/dev/null

mkdir /etc/pwnctl 2>/dev/null
mv /opt/pwnctl/config.ini /etc/pwnctl

curl https://raw.githubusercontent.com/aristosMiliaressis/pwnctl/master/src/pwnwrk.svc/scripts/get-psl.sh | bash

echo "All done!"
echo "Remember to setup your connection string in /etc/pwnctl/config.ini"
