#!/bin/bash

url=$1

mkdir -p /mnt/efs/screenshots/ 2>/dev/null

gowitness single --disable-logging --disable-db -F --user-agent "$(uagen)" -P /mnt/efs/screenshots/ $url