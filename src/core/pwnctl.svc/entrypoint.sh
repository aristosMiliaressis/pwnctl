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

if ! test -f "/mnt/efs/public_suffix_list.dat"; 
then
    get-psl.sh /mnt/efs
fi

# if no resolvers list generate new one
if [ ! -f "/mnt/efs/resolvers_top25.txt" ] 
then
    get-valid-resolvers.sh 2>&1 >/dev/null
    cp /opt/wordlists/dns/resolvers_top25.txt /mnt/efs/resolvers_top25.txt
# else if list is older than 6 hours take it but move it out of efs so next task will have to regenerate it
elif [ $(((`date +%s` - `stat -L --format %Y /mnt/efs/resolvers_top25.txt`))) -gt $((60*60*6)) ]
then
    mv /mnt/efs/resolvers_top25.txt /opt/wordlists/dns/resolvers_top25.txt
else
    cp /mnt/efs/resolvers_top25.txt /opt/wordlists/dns/resolvers_top25.txt
fi

echo "pwnctl service started on $HOSTNAME" | notify -provider discord -id status

/opt/pwnctl-svc/pwnsvc

echo "pwnctl service stoped on $HOSTNAME" | notify -provider discord -id status
