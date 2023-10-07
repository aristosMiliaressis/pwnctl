#!/bin/bash

url=$1
shift
args=( "$@" )

nuclei -j -silent -nc ${args[@]} -u $url \
    | jq -c '."matched-at" as $asset | {template:."template-id",severity:.info.severity,matcher:."matcher-name",extracted:."extracted-results"} | {asset:($asset), tags:{"nuclei-'$(echo $line | md5sum | cut -d ' ' -f1 | tr -d '\n')'":(.|tostring)}}'
