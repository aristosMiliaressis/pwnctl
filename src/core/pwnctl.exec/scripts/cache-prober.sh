#!/bin/bash

url=$1

cache-prober --log silent --parallel 8 --rps 16 --max-time 60m -u $url \
    | jq -c 'select( .Type == "vuln" ) as $vuln | {asset:"'$url'", tags:{"cache-prober-'$(echo $line | md5sum | cut -d ' ' -f1 | tr -d '\n')'":(.|tostring)}}'

