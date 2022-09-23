#!/bin/bash

# TODO add http support

url=$(echo $1 | sed 's/^tcp:/https:/g')
temp=`mktemp`

cd /opt/IIS-ShortName-Scanner/
java -jar iis_shortname_scanner.jar 2 20 $url > $temp

cat $temp | grep 'Result: Vulnerable' >/dev/null && echo "{\"Asset\":\"$url\", \"tags\":{\"shortname-misconfig\":\"true\"}}"

rm $temp