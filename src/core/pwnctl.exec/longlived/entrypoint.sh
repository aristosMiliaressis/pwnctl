#!/bin/bash

cp -R $PWNCTL_FS_MOUNT_POINT/config $HOME/.config

if test -f "$PWNCTL_FS_MOUNT_POINT/.gau.toml";
then
    cp "$PWNCTL_FS_MOUNT_POINT/.gau.toml" $HOME/.gau.toml
fi

cp $PWNCTL_FS_MOUNT_POINT/resolvers.txt /opt/wordlists/dns/resolvers.txt
cp $PWNCTL_FS_MOUNT_POINT/trusted-resolvers.txt /opt/wordlists/dns/trusted-resolvers.txt

exec /opt/pwnctl-exec/executor
