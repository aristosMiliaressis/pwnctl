#!/bin/bash

if test -f "$PWNCTL_FS_MOUNT_POINT/provider-config.yaml";
then
    mkdir -p $HOME/.config/notify/
    cp "$PWNCTL_FS_MOUNT_POINT/provider-config.yaml" $HOME/.config/notify/provider-config.yaml
fi

exec /opt/pwnctl-proc/processor
