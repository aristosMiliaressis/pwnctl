#!/bin/bash

url=$1
temp=`mktemp`

webanalyze -apps /opt/technologies.json -search -host $1 -output json 1> $temp 2>/dev/null

tags=$(cat $temp | jq -r -c '.matches[] | "\"\(.app.category_names[0])\": \"\(.app_name)\""' | tr '\n' ',')

echo "{\"Asset\":\"$url\",\"Tags\":{${tags::-1}}}"