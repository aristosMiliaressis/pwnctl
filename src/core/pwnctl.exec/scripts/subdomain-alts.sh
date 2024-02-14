#!/bin/bash
set -ux

TRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers-best.txt'
UNTRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers.txt'

if [[ $# -lt 2 ]]
then
    echo "USAGE: $0 <domain> <alt_wordlist>"
    exit 1
fi

curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > $UNTRUSTED_RESOLVERS

domain=$1
alt_wordlist=$2

tmpdir=`mktemp -d -p /tmp subdomain-alts-XXXX`
trap "rm -rf $tmpdir" EXIT
cd $tmpdir
mkdir $tmpdir/logs

jq -r '.[].Name' /dev/stdin > previously-found.tmp

while true 
do
    # merge all subdomain lists to generate alterations
    cat previously-found.tmp resolving-filtered.txt wildcards.tmp > all.tmp
    
    # generate alterations
    gotator -silent -sub all.tmp -perm $alt_wordlist -adv -depth 1 -numbers 4 > gotator.out
    python3 /opt/tools/regulator/main.py -t $domain -f all.tmp -o regulator.out
    
    # filter already queried subs & deduplicate alterations
    cat regulator.out gotator.out | sort -u | anew -d resolved.tmp > alts.tmp 

    # filter out wildcards from puredns input
    wildcard_filter=$(cat wildcards.tmp | tr '\n' '|' | sed 's/|$//' | sed 's/^/\./' | sed 's/|/|\./g' | sed 's/\./\\\./g')
    if [[ -z $wildcard_filter ]]
    then
        cat alts.tmp |awk '{$1=$1};1' > alts_clean.tmp
    else
        cat alts.tmp | grep -vE "$wildcard_filter" |awk '{$1=$1};1' > alts_clean.tmp
    fi

    # resolve alterations
    puredns resolve -q alts_clean.tmp -r $UNTRUSTED_RESOLVERS \
        --resolvers-trusted $TRUSTED_RESOLVERS \
        --write puredns.out \
        --write-wildcards wildcards2.tmp \
        --rate-limit 1500 \
        --wildcard-tests 20
    
    # filter out wildcards from puredns output
    cat wildcards2.tmp | anew -q wildcards.tmp
    wildcard_filter=$(cat wildcards.tmp | tr '\n' '|' | sed 's/|$//' | sed 's/^/\./' | sed 's/|/|\./g' | sed 's/\./\\\./g')
    if [[ -z $wildcard_filter ]]
    then
        cat puredns.out | sort -u | anew resolving-filtered.txt > new.tmp
    else
        cat puredns.out | grep -vE "$wildcard_filter" | anew resolving-filtered.txt > new.tmp
    fi

    # if no new subdomains found exit
    if ! [ -s new.tmp ]; then 
        break
    fi
done

cat resolving-filtered.txt |awk '{$1=$1};1'| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"rcode":"NOERROR"}}'
cat wildcards.tmp | xargs -I {} dig +nottlid nonexistentsubdomain.{} | tr '\t' ' ' | grep -E ' IN (A|AAAA|CNAME) ' | sed "s/nonexistentsubdomain/*/"