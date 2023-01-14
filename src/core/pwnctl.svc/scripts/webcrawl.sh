#!/bin/bash

url=$1
tmp=`mktemp`

echo $url | hakrawler -insecure -u -h "User-Agent: $(uagen)" > $tmp

katana -silent --no-sandbox -jc -hl -kf all -H "User-Agent: $(uagen)" -u $url -o $tmp 2>&1>/dev/null

cat $tmp | sort -u
rm $tmp
