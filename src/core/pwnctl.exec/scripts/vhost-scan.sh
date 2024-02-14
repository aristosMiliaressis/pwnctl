#!/bin/bash
set -eu

mkdir -p ${PWNCTL_OUTPUT_PATH} 2>/dev/null

url=$1
hostname_list=`mktemp`
output=`mktemp`
trap "rm $output $hostname_list" EXIT

while read line
do
  echo "$line"
done < /dev/stdin > $output

cat $output | jq -r '.[].TextNotation' > $hostname_list

vhost-brute -silent --only-unindexed -fc 502,503,504 -u $url -f $hostname_list > ${PWNCTL_OUTPUT_PATH}/$(echo $url | tr -d '/').json

cat ${PWNCTL_OUTPUT_PATH}/$(echo $url | tr -d '/').json | grep '{' | jq -r '. | "\(.Address)\t\(.Hostname)"'
cat ${PWNCTL_OUTPUT_PATH}/$(echo $url | tr -d '/').json | grep '{' | jq -c 'select( .WafBypass == "true" ) | {Asset:.Hostname, Tags:{WafBypass:.Address}}'
