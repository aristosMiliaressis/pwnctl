#!/bin/bash

temp=`mktemp`
keyword=$1

/opt/tools/cloud_enum/cloud_enum.py  -k $keyword -f json -l $temp >/dev/null

cat $temp | grep -v '^###' | less | jq -r .target | grep -v accounts.google.com

rm $temp