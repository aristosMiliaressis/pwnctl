#!/bin/bash

if test -f "/mnt/efs/amass.ini";
then
    cp "/mnt/efs/amass.ini" /etc/amass/config.ini
fi

if test -f "/mnt/efs/waymore.yml";
then
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

if test -f "/mnt/efs/provider-config.yaml";
then
    mkdir -p $HOME/.config/notify/
    cp "/mnt/efs/provider-config.yaml" $HOME/.config/notify/provider-config.yaml
fi

if ! test -f "/mnt/efs/public_suffix_list.dat";
then
    get-psl.sh /mnt/efs
fi

# if no resolvers list generate new one
if [ ! -f "/mnt/efs/resolvers.txt" ]
then
    get-valid-resolvers.sh
    cp /opt/wordlists/dns/resolvers.txt /mnt/efs/resolvers.txt
# else if list is older than 6 hours take it but move it out of efs so next task will have to regenerate it
elif [ $(((`date +%s` - `stat -L --format %Y /mnt/efs/resolvers.txt`))) -gt $((60*60*24)) ]
then
    mv /mnt/efs/resolvers.txt /opt/wordlists/dns/resolvers.txt
else
    cp /mnt/efs/resolvers.txt /opt/wordlists/dns/resolvers.txt
fi
cp /mnt/efs/trusted-resolvers.txt /opt/wordlists/dns/trusted-resolvers.txt

exec /opt/pwnctl-svc/pwnsvc
