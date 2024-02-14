#!/bin/bash
set -eu

url=$1

mkdir -p $PWNCTL_OUTPUT_PATH/screenshots/ || echo -n 2>/dev/null

gowitness single --delay 5 --disable-logging --disable-db -F -P $PWNCTL_OUTPUT_PATH/screenshots $url