#!/bin/bash
set -u 

if [ $# -lt 2 ]
then
    echo "USAGE: $0 <url> [wordlists1 wordlist2 ...]"
    exit 1
fi

url=$1

! $(skiphost -u $url | jq -r .looks_good) && echo "skipped host" 1>&2 && exit 0

temp_wordlist=`mktemp`
temp_outfile=`mktemp`
trap "rm $temp_outfile $temp_wordlist" EXIT

shift
for list in "$@";
do
    cat $list
done | sort -u > $temp_wordlist

ffuf -maxtime 9000 -s -o $temp_outfile -of json -se -acs waf,blacklist,wildcard,route-handler \
    -mc 200,204,401,500,501 -H "User-Agent: $(uagen)" -w $temp_wordlist -u ${url}FUZZ >/dev/null

cat $temp_outfile \
    | jq -c '.results[] | {asset: ("%%BASE_URL%%"+.input.FUZZ), tags:{status:.status|tostring,location:.redirectlocation}}' \
    | sed "s/%%BASE_URL%%/$(echo $url | sed 's,/,\\/,g')/g" \
    | while read line; do word=$(echo $line | jq -r .asset | sed "s/$(echo $url | sed 's,/,\\/,g')//"); \
        for list in "$@"; do grep -q -E "^$word\$" $list && echo $line | jq -c ".tags.source = \"$list\""; done; done

