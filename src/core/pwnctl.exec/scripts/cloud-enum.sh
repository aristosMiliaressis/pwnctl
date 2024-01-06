#!/bin/bash
set -eu

temp=`mktemp`
trap "rm $temp" EXIT

keyword=$1

cloud_enum -k $keyword -f json -l $temp \
        -m /opt/tools/src/cloud-enum/enum_tools/fuzz.txt \
        -b /opt/tools/src/cloud-enum/enum_tools/fuzz.txt >/dev/null

cat $temp | grep -v '^###' | less | jq -r .target | grep -v accounts.google.com || exit 0
