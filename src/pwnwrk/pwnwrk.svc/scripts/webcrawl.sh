#!/bin/bash

url=$1
tmp=`mktemp`

echo $url | hakrawler -insecure -u -h "User-Agent: $(uagen)" > $tmp

katana -silent -jc -hl -kf all -H "User-Agent: $(uagen)" -u $url >> $tmp 2>/dev/null

cat $tmp | sort -u
rm $tmp
