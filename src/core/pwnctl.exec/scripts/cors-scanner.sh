#!/bin/bash
set -eux

url=$1

timeout --preserve-status -v -k 1m 15m cors-scanner -u $url \
    | jq -c '{asset:"'$url'", tags:{("cors-"+.Name):(.|tostring)}}'
