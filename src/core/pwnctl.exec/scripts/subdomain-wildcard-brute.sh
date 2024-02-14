#!/bin/bash
set -ux

RESOLVERS='/opt/wordlists/dns/resolvers.txt'
TRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers-best.txt'
COUNT=50

if [ $# -lt 2 ]
then
	echo "USAGE: $0 <domain> <wordlist>"
	exit 2
fi

domain=$1
wordlist=$2

tmpdir=`mktemp -d -p /tmp wildcard-brute-XXXX`
trap "rm -rf $tmpdir" EXIT

curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > $RESOLVERS

# create profile of wildcard records.
# multiple tries to capture all records in case of DNS load balancing
for i in $(seq 0 $COUNT)
do
	echo "$(cat /proc/sys/kernel/random/uuid).$domain"
done | massdns -q -r $TRUSTED_RESOLVERS -o Snl -t A -s 250 \
	--root --retry REFUSED --retry SERVFAIL \
	| grep -oE '([0-9]{1,3}\.){3}[0-9]{1,3}$' | sort -u > $tmpdir/profile.tmp

# generate FQDNs from subdomain wordlist
awk 'NF{print $0 ".'$domain'"}' $wordlist > $tmpdir/fqdn.tmp

# resolve FQDNs
massdns -q -r $RESOLVERS -o Snl -t A --root -s 1500 \
	--retry REFUSED --retry SERVFAIL -w $tmpdir/records.tmp $tmpdir/fqdn.tmp

# filter profiled responses
profile_filter="$(cat $tmpdir/profile.tmp | tr '\n' '|' | sed 's/|$//' | sed 's/\./\\\./g')$"
cat $tmpdir/records.tmp \
	| grep . \
	| grep -vE "$profile_filter" \
	| cut -d ' ' -f 1 \
	| sort -u > $tmpdir/new_records.tmp

# filter poisoned responses from untrusted resolvers
massdns -q -r $TRUSTED_RESOLVERS -o Snl -t A -s 250 \
	--root --retry REFUSED --retry SERVFAIL $tmpdir/new_records.tmp \
	| grep . \
	| grep -vE "$profile_filter" \
	| cut -d ' ' -f 1 \
	| sort -u | grep $domain
