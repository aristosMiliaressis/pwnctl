#!/bin/bash

url=$1

mdwfuzzer -s -j -d 0.1 -u $url \
    | jq -c ". | {asset:\"$url\",tags:{mdwfuzzer_check:.check,mdwfuzzer_word:.word}}" 2>/dev/null