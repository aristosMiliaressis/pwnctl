#!/bin/bash
set -eux

url=$1

! $(skiphost -u $url | jq -r .looks_good) && echo "skipped host" 1>&2 && exit 0

mdwfuzzer -s -j -d 0.1 -u $url \
    | jq -c ". | {asset:\"$url\",tags:{mdwfuzzer_check:.check,mdwfuzzer_word:.word}}" 2>/dev/null