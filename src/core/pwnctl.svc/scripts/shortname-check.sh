#!/bin/bash

url=$1
temp=`mktemp`

if [[ $url == *"tcp://"* ]]
then
    url=$(echo $url | sed 's/^tcp:\/\///')
    echo | openssl s_client -connect $url -brief &>/dev/null
    if [[ $? -eq 0 ]]
    then
        url="https://$url"
    else
        url="http://$url"
    fi
fi

cd /opt/tools/IIS-ShortName-Scanner/
java -jar iis_shortname_scanner.jar 2 20 $url > $temp

cat $temp | grep 'Result: Vulnerable' >/dev/null && echo "{\"Asset\":\"$url\", \"tags\":{\"shortname-misconfig\":\"true\"}}"

rm $temp