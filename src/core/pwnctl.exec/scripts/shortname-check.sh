#!/bin/bash
set -eux

url=$1
temp=`mktemp`
trap "rm $temp" EXIT

if [[ $url == *"tcp://"* ]]
then
    url=$(echo $url | sed 's,^tcp://,,')
    echo | openssl s_client -connect $url -brief &>/dev/null
    if [[ $? -eq 0 ]]
    then
        url="https://$url"
    else
        url="http://$url"
    fi
fi

cd /opt/toos/ShortNameScanner/

echo No | java -jar iis_shortname_scanner.jar 2 20 $url \
        | grep -q 'Vulnerable!' && echo '{"Asset":"'$url'", "tags":{"shortname-misconfig":"true"}}' || exit 0
