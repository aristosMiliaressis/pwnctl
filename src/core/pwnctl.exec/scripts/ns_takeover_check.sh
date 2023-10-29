#!/bin/bash

domain=$1

python3 /opt/tools/dnsReaper/main.py single --domain $domain &> | grep -q ':: SIGNATURE' \
    && echo '{"Asset":"'$url'", "tags":{"ns-takeover":"true"}}'