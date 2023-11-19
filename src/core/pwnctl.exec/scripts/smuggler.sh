#!/bin/bash

url=$1

mkdir /mnt/efs/smuggler 2>/dev/null

python /opt/tools/smuggler/smuggler.py -q --no-color -u $url -l /mnt/efs/smuggler | grep -q "Issue Found" && echo '{"Asset":"'$url'","Tags":{"h1_smuggling":"true"}}'
