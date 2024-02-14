#!/bin/bash
set -eu 

url=$1

nuclei -silent -nc -t /opt/nuclei_templates/discovery -u $url \
    | jq -c --raw-input '. | split(" ") | {Asset:(.[3]),Tags:{"exposed-api":(.[0])}}'
