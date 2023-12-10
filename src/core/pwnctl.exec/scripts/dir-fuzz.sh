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

timeout -v -k 30s 150m ffuf -maxtime 9000 -s -o $temp_outfile -of json -se -fp -acp -acs waf,blacklist,wildcard,route-handler \
    -ar -mc all -fc 404,429,503 -recursion -recursion-status 301,302,303,307,308,401,403,405,500,501 -H "User-Agent: $(uagen)" -w $temp_wordlist -u ${url}FUZZ >/dev/null

cat $temp_outfile \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status|tostring,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | 's,/,\\/,g')/g" \
    | while read line; do word=$(echo $line | jq -r .asset | sed "s/$(echo $url | sed 's,/,\\/,g')//"); \
        for list in "$@"; do grep -q -E "^$word\$" $list && echo $line | jq -c ".tags.source = \"$list\""; done; done

