#!/bin/bash

url=$1

nuclei -silent -nc -t /opt/nuclei_templates/http/exposed-panels -u $url \
    | jq -c --raw-input '. | split(" ") | {Asset:(.[3]),Tags:{"exposed-panel":(.[0])}}'