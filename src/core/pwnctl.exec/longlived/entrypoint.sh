#!/bin/bash

mkdir -p $HOME/.config
cp -R $PWNCTL_FS_MOUNT_POINT/config/* $HOME/.config

if test -f "$PWNCTL_FS_MOUNT_POINT/.gau.toml";
then
    cp "$PWNCTL_FS_MOUNT_POINT/.gau.toml" $HOME/.gau.toml
fi

. /root/.env

exec /opt/pwnctl-exec/executor
