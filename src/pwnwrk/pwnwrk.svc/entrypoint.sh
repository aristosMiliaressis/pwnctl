#!/bin/bash

cp /mnt/efs/config.ini $PWNCTL_InstallPath

# volume mapped entrypoint_hook.sh for injecting resources and 
# running commands at startup without needing to rebuild the image
if test -f "/mnt/efs/entrypoint_hook.sh"; 
then
    chmod +x /mnt/efs/entrypoint_hook.sh
    /mnt/efs/entrypoint_hook.sh
fi

if ! test -f "/mnt/efs/public_suffix_list.dat"; 
then
    get-psl.sh /mnt/efs
fi

# if the resolver list is older than 6 hours replace it with a new one
if [ ! -f "/mnt/efs/resolvers_top25.txt" ] || [ $(((`date +%s` - `stat -L --format %Y /mnt/efs/resolvers_top25.txt`))) -gt $((60*60*6)) ]
then
    echo "Getting fresh resolvers"
    get-valid-resolvers.sh 2>&1 >/dev/null
    cp /opt/wordlists/dns/resolvers_top25.txt /mnt/efs/resolvers_top25.txt
else
    cp /mnt/efs/resolvers_top25.txt /opt/wordlists/dns/resolvers_top25.txt
fi

echo "pwnwrk service started on $HOSTNAME" | notify -provider discord -id status

/opt/pwnwrk-svc/pwnwrk

echo "pwnwrk service stoped on $HOSTNAME" | notify -provider discord -id status
