#!/bin/bash

url=$1

mkdir -p /mnt/efs/screenshots 2>/dev/null

echo $url | httpx -silent -ss -srd /mnt/efs/screenshots
