#!/bin/bash
set -eu 

url=$1

mkdir $PWNCTL_FS_MOUNT_POINT/smuggler || echo -n 2>/dev/null

python /opt/tools/smuggler/smuggler.py -q --no-color -u $url -l $PWNCTL_FS_MOUNT_POINT/smuggler \
    | grep -q "Issue Found" \
    && echo '{"Asset":"'$url'","Tags":{"h1_smuggling":"true"}}' || exit 0