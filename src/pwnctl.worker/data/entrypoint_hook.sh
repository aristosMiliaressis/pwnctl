#!/bin/bash

if test -f "$PWNCTL_INSTALL_PATH/amass.ini"; 
then
    cp "$PWNCTL_INSTALL_PATH/amass.ini" /etc/amass/config.ini
fi

if test -f "$PWNCTL_INSTALL_PATH/aws.config"; 
then
    mkdir ~/.aws
    cp "$PWNCTL_INSTALL_PATH/aws.config" ~/.aws/config
fi

if test -f "$PWNCTL_INSTALL_PATH/aws.credentials"; 
then
    cp "$PWNCTL_INSTALL_PATH/aws.credentials" ~/.aws/credentials
fi

if test -f "$PWNCTL_INSTALL_PATH/provider-config.yaml"; 
then
    mkdir -p $HOME/.config/notify/
    cp "$PWNCTL_INSTALL_PATH/provider-config.yaml" $HOME/.config/notify/provider-config.yaml

    /sbin/service cron start

    env | grep PWNCTL_ | xargs -I _ echo "export _" >> /etc/profile
    echo 'export PATH=$PATH:/usr/local/bin:/root/go/bin/' >> /etc/profile
    (crontab -l ; echo "00 */2 * * * BASH_ENV=/etc/profile /usr/local/bin/notify-status.sh") | crontab
fi

