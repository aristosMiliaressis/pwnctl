#!/bin/bash

uri=$1
temp=`mktemp`

xnLinkFinder -insecure -d 1 -i $uri -sp $uri -u 'desktop' -o $temp -vv > /dev/null 2>&1;

cat $temp

rm $temp