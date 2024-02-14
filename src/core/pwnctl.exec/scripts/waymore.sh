#!/bin/bash
set -ux

domain=$1

mkdir -p ${PWNCTL_OUTPUT_PATH}/$domain 2>/dev/null

timeout --preserve-status -k 2m 160m waymore.py -lcc 1 -c /root/.config/waymore/config.yml -i $domain \
    -oU ${PWNCTL_OUTPUT_PATH}/$domain/urls.txt -oR ${PWNCTL_OUTPUT_PATH}/$domain/results -ci m -from 2017 -l 1800 -lr 4000 &>/dev/null

xnLinkFinder.py --no-banner --scope-filter $domain --config /root/.config/xnLinkFinder/config.yml \
    -i ${PWNCTL_OUTPUT_PATH}/$domain/results -owl  ${PWNCTL_OUTPUT_PATH}/$domain/wordlist.txt -sp $domain >/dev/null