#!/bin/bash

if [ $# -lt 2 ]
then
    echo "USAGE: $0 <url> [wordlists1 wordlist2 ...]"
    exit 1
fi

url=$1
temp_wordlist=`mktemp`
temp_outfile=`mktemp`
trap "rm $temp_outfile $temp_wordlist" EXIT

shift
for list in "$@";
do
    cat $list
done | sort -u > $temp_wordlist

ffuf -s -o $temp_outfile -of json -se -acp -acs advanced,waf,blacklist,wildcard,route-handler -ar -mc all \
    -recursion -recursion-status 301,302,303,307,308,401,403 -H "User-Agent: $(uagen)" -w $temp_wordlist -u ${url}FUZZ &>/dev/null

cat $temp_outfile \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's/\//\\\//g')/g" \
    | while read line; do word=$(echo $line | jq -r .asset | sed "s/$(echo $url | sed 's/\//\\\//g')//"); \
        for list in "$@"; do cat $list | grep -q -E "^$word\$" && echo $line | jq -c ".tags.source = \"$list\""; done; done

