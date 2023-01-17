#!/bin/bash

if test -f "/mnt/efs/amass.ini"; 
then
    cp "/mnt/efs/amass.ini" /etc/amass/config.ini
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
if [ ! -f "/mnt/efs/resolvers_top25.txt" ] 
then
    get-valid-resolvers.sh >> "${PWNCTL_Logging__FilePath}/pwnctl.log"
    cp /opt/wordlists/dns/resolvers_top25.txt /mnt/efs/resolvers_top25.txt
# else if list is older than 6 hours take it but move it out of efs so next task will have to regenerate it
elif [ $(((`date +%s` - `stat -L --format %Y /mnt/efs/resolvers_top25.txt`))) -gt $((60*60*24)) ]
then
    mv /mnt/efs/resolvers_top25.txt /opt/wordlists/dns/resolvers_top25.txt
else
    cp /mnt/efs/resolvers_top25.txt /opt/wordlists/dns/resolvers_top25.txt
fi

exec /opt/pwnctl-svc/pwnsvc
