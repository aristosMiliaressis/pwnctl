#!/bin/bash

if test -f "/mnt/efs/amass.ini"; 
then
    cp "/mnt/efs/amass.ini" /etc/amass/config.ini
fi

if test -f "/mnt/efs/provider-config.yaml"; 
then
    mkdir -p $HOME/.config/notify/
    cp "/mnt/efs/provider-config.yaml" $HOME/.config/notify/provider-config.yaml

    /sbin/service cron start

    env | grep PWNCTL_ | xargs -I _ echo "export \"_\"" >> /etc/profile
    echo 'export PATH=$PATH:/usr/local/bin:/root/go/bin/' >> /etc/profile
    (crontab -l ; echo "00 */2 * * * BASH_ENV=/etc/profile /usr/local/bin/notify-status.sh") | crontab
fi

