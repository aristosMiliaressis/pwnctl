#!/bin/bash
set -eux 

url=$1

nuclei -silent -nc -t /opt/nuclei_templates/discovery/swagger-ui.yaml -u $url \
    | grep '^\[swagger-ui\]' \
    | awk '{print $NF}' \
    | jq -c --raw-input '{Asset:(.), Tags:{"swagger-ui":"true"}}'