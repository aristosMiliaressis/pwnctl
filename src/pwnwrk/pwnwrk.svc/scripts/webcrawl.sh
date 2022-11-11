#!/bin/bash

url=$1
tmp=`mktemp`

echo $url | hakrawler -insecure -u -h "User-Agent: $(uagen)" > $tmp

katana -silent -jc -kf all -H "User-Agent: $(uagen)" -u $url >> $tmp

cat $tmp | sort -u
rm $tmp
