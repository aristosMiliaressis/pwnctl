#!/bin/bash
set -eux

if [ $# -lt 2 ]
then
    echo "USAGE: $0 <url> [lowercase: true|false] [wordlists1 wordlist2 ...]"
    exit 1
fi

url=$1
lowercase=$2
shift
shift

! $(skiphost -u $url | jq -r .looks_good) && echo "skipped host" 1>&2 && exit 0

temp_wordlist=`mktemp`
temp_outfile=`mktemp`
trap "rm $temp_outfile $temp_wordlist" EXIT

for list in "$@";
do
    [[ -f $(ls $list) ]] && cat $list
done > $temp_wordlist

if [[ "$lowercase" == 'true' ]]
then
    sed -i -e "s/\(.*\)/\L\1/g" $temp_wordlist
fi

wordlist_cleaner.sh $temp_wordlist

mkdir -p $PWNCTL_OUTPUT_PATH
ffuf -maxtime 9000 -s -o $temp_outfile -of json -se -acs waf,blacklist,wildcard,route-handler \
    -od $PWNCTL_OUTPUT_PATH -recursion -recursion-depth 1 -mc 200 -H "User-Agent: $(uagen)" -w $temp_wordlist -u ${url}FUZZ >/dev/null

clean=$(cat $temp_outfile | jq -c '.results[]' | ffufClean) 

echo $clean \
    | jq -c '.[] | {asset:.url,tags:{status:.status|tostring,location:.redirectlocation}}' \
    | while read line; do \
        word=$(echo $line | jq -r .asset | sed "s/$(echo $url | sed 's,/,\\/,g')//"); \
        for list in "$@"; do \
            grep -q -iE "^$word\$" $list && echo $line | jq -c ".tags.source = \"$list\""; \
        done | head -n 1; \
    done
