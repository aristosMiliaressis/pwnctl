#!/bin/bash

url=$1
tmp=`mktemp`
tmp2=`mktemp`

katana -silent --no-sandbox -jc -hl -kf all -u $url -o $tmp >/dev/null

echo $url | hakrawler -insecure -u -h "User-Agent: $(uagen)" >> $tmp

python3 /opt/tools/xnLinkFinder/xnLinkFinder.py -sf $url -insecure -d 1 -i $url -sp $url -u 'desktop' -o $tmp2 -vv > /dev/null

cat $tmp2 >> $tmp
cat $tmp | sort -u

rm $tmp2
rm $tmp
