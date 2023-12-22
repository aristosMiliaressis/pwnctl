#!/bin/bash
set -eux

temp=`mktemp`
trap "rm $temp" EXIT

keyword=$1

cloud_enum -k $keyword -f json -l $temp >/dev/null

cat $temp | grep -v '^###' | less | jq -r .target | grep -v accounts.google.com || exit 0
