#!/bin/bash

url=$1
temp=`mktemp`

webanalyze -apps /opt/tools/technologies.json -crawl 1 -search -host $url -output json 1> $temp 2>/dev/null

tags=$(cat $temp | jq -r -c '.matches[] | "\"\(.app.category_names[0])\": \"\(.app_name)\""' | tr '\n' ',' | head -c -1)

echo "{\"Asset\":\"$url\",\"Tags\":{$tags}}" 2>/dev/null