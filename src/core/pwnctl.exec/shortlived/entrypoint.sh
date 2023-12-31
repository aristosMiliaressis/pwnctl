#!/bin/bash

mkdir -p $HOME/.config
cp -R $PWNCTL_FS_MOUNT_POINT/config/* $HOME/.config

. /root/.env

exec /opt/pwnctl-exec/executor
