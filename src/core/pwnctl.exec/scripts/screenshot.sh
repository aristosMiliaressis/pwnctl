#!/bin/bash

url=$1

mkdir -p $PWNCTL_FS_MOUNT_POINT/screenshots/ 2>/dev/null

gowitness single --disable-logging --disable-db -F --user-agent "$(uagen)" -P $PWNCTL_FS_MOUNT_POINT/screenshots/ $url