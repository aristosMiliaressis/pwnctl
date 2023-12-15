#!/bin/bash

if test -f "/mnt/efs/provider-config.yaml";
then
    mkdir -p $HOME/.config/notify/
    cp "/mnt/efs/provider-config.yaml" $HOME/.config/notify/provider-config.yaml
fi

exec /opt/pwnctl-proc/processor
