#!/bin/bash
set -ux

TRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers-best.txt'
UNTRUSTED_RESOLVERS='/opt/wordlists/dns/resolvers.txt'

do_passive_enum() {
	assetfinder --subs-only $1 | grep -iE "^[a-z0-9\-_\.]+\.$1" | anew -q $2
	amass enum -silent -passive -nocolor -d $1 | anew -q $2
	subfinder -d $1 -silent -max-time 2 | anew -q $2
	if [[ -f $HOME/.config/github.tokens ]]
	then 
		github-subdomains -raw -d $1 -t $HOME/.config/github.tokens | anew -q $2
	fi
	if [[ $3 == 'initial' && -f $HOME/.config/shodan.token ]]
	then
		shodan search --no-color --fields hostnames "ssl.cert.subject.cn:$1" \
			| tr ';' '\n' \
			| anew -q $2
	fi
}

get_highly_nested_subs() {
	cat $1 \
		| rev \
		| cut -d '.' -f 3,2,1 \
		| rev \
		| sort \
		| uniq -c \
		| sort -nr \
		| grep -v '1 ' \
		| head -n 10 \
		| sed -e 's/^[[:space:]]*//' \
		| cut -d ' ' -f 2

	cat $1 \
		| rev \
		| cut -d '.' -f 4,3,2,1 \
		| rev \
		| sort \
		| uniq -c \
		| sort -nr \
		| grep -v '1 ' \
		| head -n 10 \
		| sed -e 's/^[[:space:]]*//' \
		| cut -d ' ' -f 2
}

if [[ $# -lt 1 ]]
then
    echo "USAGE: $0 <domain>"
    exit 1
fi

curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > $UNTRUSTED_RESOLVERS

domain=$1

tmpdir=`mktemp -d -p /tmp subdomain-osint-XXXX`
trap "rm -rf $tmpdir" EXIT
cd $tmpdir

[[ -f $HOME/.config/shodan.token ]] && shodan init $(cat $HOME/.config/shodan.token) &>/dev/null

do_passive_enum $domain passive-subs.tmp 'initial'
for sub in $(get_highly_nested_subs passive-subs.tmp | sort -u);
do 
    do_passive_enum $sub recursive-passive.tmp ''
done

cat passive-subs.tmp recursive-passive.tmp | sort -u | grep -v 'No assets were discovered' > unresolved.tmp

puredns resolve -q unresolved.tmp -r $UNTRUSTED_RESOLVERS \
		--resolvers-trusted $TRUSTED_RESOLVERS \
		--write puredns.out \
		--write-wildcards wildcards.tmp \
		--rate-limit 1500 \
		--wildcard-tests 20 >/dev/null
        
cat puredns.out |awk '{$1=$1};1'| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"rcode":"NOERROR"}}'
cat passive-subs.tmp | anew -d puredns.out |awk '{$1=$1};1'| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"rcode":"NXDOMAIN"}}'

echo '{"Asset":"'$domain'","Tags":{"osint":"done"}}'