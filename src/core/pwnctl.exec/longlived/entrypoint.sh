#!/bin/bash

cp -R /mnt/efs/config $HOME/.config

if test -f "/mnt/efs/.gau.toml";
then
    cp "/mnt/efs/.gau.toml" $HOME/.gau.toml
fi

cp /mnt/efs/resolvers.txt /opt/wordlists/dns/resolvers.txt
cp /mnt/efs/trusted-resolvers.txt /opt/wordlists/dns/trusted-resolvers.txt

exec /opt/pwnctl-exec/executor
