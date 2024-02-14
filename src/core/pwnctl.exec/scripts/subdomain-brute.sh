#!/bin/bash
set -ux

TRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers-best.txt'
UNTRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers.txt'

if [[ $# -lt 2 ]]
then
    echo "USAGE: $0 <domain> <wordlist>"
    exit 1
fi

curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > $UNTRUSTED_RESOLVERS

domain=$1
wordlist=$2

tmpdir=`mktemp -d -p /tmp subdomain-brute-XXXX`
trap "rm -rf $tmpdir" EXIT
cd $tmpdir

# generate subdomain guesses
awk 'NF{print $0 ".'$domain'"}' $wordlist | anew -q unresolved.tmp

# resolve subdomain guesses
puredns resolve -q unresolved.tmp -r $UNTRUSTED_RESOLVERS \
    --resolvers-trusted $TRUSTED_RESOLVERS \
    --write puredns.out \
    --write-wildcards wildcards.tmp \
    --rate-limit 1500 \
    --wildcard-tests 20

# filter out wildcards from puredns output
wildcard_filter=$(cat wildcards.tmp | tr '\n' '|' | sed 's/|$//' | sed 's/^/\./' | sed 's/|/|\./g' | sed 's/\./\\\./g')
if [[ -z $wildcard_filter ]]
then
    cat puredns.out |awk '{$1=$1};1'| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"rcode":"NOERROR"}}'
else
    cat puredns.out | grep -vE "$wildcard_filter" |awk '{$1=$1};1'| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"rcode":"NOERROR"}}'
fi

cat wildcards.tmp | xargs -I {} dig +nottlid nonexistentsubdomain.{} | tr '\t' ' ' | grep -E ' IN (A|AAAA|CNAME) ' | sed "s/nonexistentsubdomain/*/"

echo '{"Asset":"'$domain'","Tags":{"brute":"done"}}'