#!/bin/bash

url=$1
hostname_list=`mktemp`
output=`mktemp`
trap "rm $output $hostname_list" EXIT

while read line
do
  echo "$line"
done < "${1:-/dev/stdin}" | jq -r '.[].TextNotation' > $hostname_list

vhost-brute -silent --only-unindexed -fc 502,503,504 -u $url -f $hostname_list > $output

cat $output | jq -c 'select( .WafBypass == "true" ) | {Asset:.Hostname, Tags:{WafBypass:.Address}}'
cat $output | jq -r '. | "\(.Hostname) IN VHOST \(.Address)"'
