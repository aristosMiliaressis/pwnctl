#!/bin/bash

url=$1
temp=`mktemp`

java -jar /opt/IIS-ShortName-Scanner/iis_shortname_scanner.jar 2 20 $url > $temp

cat $temp | grep 'Result: Vulnerable' >/dev/null && echo "{\"Asset\":\"$url\", \"tags\":{\"shortname-misconfig\":\"true\"}}"

rm $temp