#!/bin/bash

cp -R $PWNCTL_FS_MOUNT_POINT/config $HOME/.config

. /root/.profile

exec /opt/pwnctl-exec/executor
