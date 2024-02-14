#!/bin/bash
set -eu 

url=$1

mkdir $PWNCTL_OUTPUT_PATH 2>/dev/null || echo -n

python /opt/tools/smuggler/smuggler.py -q --no-color -u $url -l $PWNCTL_OUTPUT_PATH/$(echo $url | tr -d '/').log \
    | grep -q "Issue Found" \
    && echo '{"Asset":"'$url'","Tags":{"h1_smuggling":"true"}}' || exit 0