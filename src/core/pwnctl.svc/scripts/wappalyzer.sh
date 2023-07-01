#!/bin/bash
set -euo pipefail 

url=$1

tags=$(wappy -u $url | grep -v exception \
    | grep . \
    | tail -n +2 \
    | while read line; \
    do \
        key=$(echo $line | cut -d : -f 1|xargs); \
        value=$(echo $line | cut -d : -f 2 | cut -d '[' -f 1|xargs); \
        echo "\"$key\":\"$value\""; \
    done \
    | tr '\n' ',' \
    | head -c -1)

echo '{"Asset":"'$url'","Tags":{'$tags'}}'