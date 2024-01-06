#!/bin/bash
set -eux

domain=$1

timeout --preserve-status -k 1m 150m /opt/tools/src/waymore/waymore.py -lcc 1 -c /root/.config/waymore/config.yml -mode U -i $domain \
    | urless -fk jpg,png,svg,jpeg,gif,ico,ttf,woff,woff2,eot,css,pdf