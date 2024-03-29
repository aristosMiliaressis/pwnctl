#!/bin/bash
set -eu

domain=$1

grep -hirE "\b$domain\b" /opt/wordlists/cloud/ \
        | tr -d '][' \
        | while read line; do \
                addr=$(echo $line | cut -d ' ' -f1); \
                hostnames=$(echo $line | cut -d ' ' -f 3-); \
                echo $hostnames | tr ' ' '\n' | awk "{print \"$addr\t\" \$0}"; \
        done | sed 's/*\.//' | sort -u