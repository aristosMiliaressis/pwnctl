#!/bin/bash

url=$1
temp=`mktemp`

if [[ ! -f technologies.json ]]
then
    webanalyze -update 2>/dev/null
elif [ $(((`date +%s` - `stat -L --format %Y technologies.json`))) -gt $((60*60*24)) ]
then
    webanalyze -update 2>/dev/null
fi

webanalyze -crawl 3 -search -host $url -output json > $temp 2>/dev/null

tags=$(cat $temp | jq -r -c '.matches[] | "\"\(.app.category_names[0])\": \"\(.app_name)\""' | sort -u | tr '\n' ',' | head -c -1)

echo '{"Asset":"'$url'","Tags":{'$tags'}}' 2>/dev/null
