#!/bin/bash
set -eux

url=$1
shift
args=( "$@" )

nuclei -j -silent -nc ${args[@]} -u $url \
    | jq -c '."matched-at" as $asset | {template:."template-id",severity:.info.severity,matcher:."matcher-name",extracted:."extracted-results"} | {asset:($asset), tags:{"nuclei-'$(head -c 18 /dev/random | base32 | tr -d '\n')'":(.|tostring)}}'
