#!/bin/bash

url=$1
webanalyze_out=`mktemp`
wappalyzer_out=`mktemp`

trap "rm $webanalyze_out $wappalyzer_out" EXIT

if [[ ! -f technologies.json ]]
then
    webanalyze -silent -update
elif [ $(((`date +%s` - `stat -L --format %Y technologies.json`))) -gt $((60*60*24)) ]
then
    webanalyze -silent -update
fi

webanalyze -silent -apps technologies.json -crawl 3 -search -host $url -output json > $webanalyze_out

tags=$(cat $webanalyze_out | jq -r -c '.matches[] | "\"\(.app.category_names[0])\": \"\(.app_name)\""' | sort -u | tr '\n' ',' | head -c -1)

echo '{"Asset":"'$url'","Tags":{'$tags'}}' > $webanalyze_out


cd /opt/tools/src/wappybird/wappybird/

python main.py -q -wf $wappalyzer_out -u $url >/dev/null

tags=$(cat $wappalyzer_out \
    | tail -n +2 | cut -d , -f 3- | tr ',' '\t' \
    | awk -F '\t' '{print "\""$1"\":\""$2"\",\""$1" version\":\""$3"\""}' \
    | tr ',' '\n' | grep -v unknown | tr '\n' , | head -c -1)

echo '{"Asset":"'$url'","Tags":{'$tags'}}' > $wappalyzer_out

jq -c -s '.[0] * .[1]' $webanalyze_out $wappalyzer_out
