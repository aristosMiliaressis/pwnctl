#!/bin/bash

temp=`mktemp`
keyword=$1
RESOLVERS=$(cat /opt/wordlists/dns/resolvers_top25.txt| tr '\n' ',')

/opt/tools/cloud_enum/cloud_enum.py -ns $RESOLVERS -k $keyword -f json -l $temp >/dev/null

cat $temp | grep -v '^###' | less | jq -r .target | grep -v accounts.google.com

rm $temp