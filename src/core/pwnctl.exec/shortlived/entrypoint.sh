#!/bin/bash

cp -R /mnt/efs/config $HOME/.config

cp /mnt/efs/resolvers.txt /opt/wordlists/dns/resolvers.txt
cp /mnt/efs/trusted-resolvers.txt /opt/wordlists/dns/trusted-resolvers.txt

exec /opt/pwnctl-exec/executor
