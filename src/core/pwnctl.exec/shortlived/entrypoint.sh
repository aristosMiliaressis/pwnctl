#!/bin/bash

if test -f "/mnt/efs/provider-config.yaml";
then
    mkdir -p $HOME/.config/notify/
    cp "/mnt/efs/provider-config.yaml" $HOME/.config/notify/provider-config.yaml
fi

cp /mnt/efs/resolvers.txt /opt/wordlists/dns/resolvers.txt
cp /mnt/efs/trusted-resolvers.txt /opt/wordlists/dns/trusted-resolvers.txt

exec /opt/pwnctl-exec/executor
