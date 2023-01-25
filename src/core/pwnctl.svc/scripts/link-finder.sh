#!/bin/bash

uri=$1
temp=`mktemp`

xnLinkFinder -insecure -d 1 -i $uri -sp $uri -u 'desktop' -o $temp -vv > /dev/null

cat $temp

rm $temp