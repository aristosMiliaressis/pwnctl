#!/bin/bash

if test -f "$PWNCTL_INSTALL_PATH/amass.ini"; 
then
    cp "$PWNCTL_INSTALL_PATH/amass.ini" /etc/amass/config.ini
fi

if test -f "$PWNCTL_INSTALL_PATH/provider-config.yaml"; 
then
    mkdir -p $HOME/.config/notify/
    cp "$PWNCTL_INSTALL_PATH/provider-config.yaml" $HOME/.config/notify/provider-config.yaml

    /sbin/service cron start

    env | grep PWNCTL_ | xargs -I _ echo "export _" >> /etc/profile
    echo 'export PATH=$PATH:/usr/local/bin:/root/go/bin/' >> /etc/profile
    (crontab -l ; echo "00 * * * * BASH_ENV=/etc/profile /usr/local/bin/notify-status.sh") | crontab
fi

if ! test -f "/opt/wordlists/dns/public_suffix_list.dat"; 
then
    get-psl.sh
fi

echo 'get-valid-resolvers.sh' | job-queue.sh -w 1 -q "$PWNCTL_INSTALL_PATH/jobs"
