#!/bin/bash

if test -f "/mnt/efs/provider-config.yaml";
then
    mkdir -p $HOME/.config/notify/
    cp "/mnt/efs/provider-config.yaml" $HOME/.config/notify/provider-config.yaml
fi

if test -f "/mnt/efs/amass.ini";
then
    mkdir -p /etc/amass/
    cp "/mnt/efs/amass.ini" /etc/amass/config.ini
fi

if test -f "/mnt/efs/waymore.yml";
then
    mkdir -p /opt/tools/waymore/
    cp "/mnt/efs/waymore.yml" /opt/tools/waymore/config.yml
fi

if test -f "/mnt/efs/.gau.toml";
then
    cp "/mnt/efs/.gau.toml" $HOME/.gau.toml
fi

if test -f "/mnt/efs/whoisxml.conf";
then
    mkdir $HOME/.config/ 2>/dev/null
    cp "/mnt/efs/whoisxml.conf" $HOME/.config/whoisxml.conf
fi

cp /mnt/efs/resolvers.txt /opt/wordlists/dns/resolvers.txt
cp /mnt/efs/trusted-resolvers.txt /opt/wordlists/dns/trusted-resolvers.txt

exec /opt/pwnctl-exec/executor
