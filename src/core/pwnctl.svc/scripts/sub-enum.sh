#!/bin/bash

# TODO: add subfinder & assetfinder & finddomain

domain=$1
dict='/opt/wordlists/subdomains-top-20000.txt'
DNS_RESOLVERS_FILE='/opt/wordlists/dns/resolvers.txt'
ALT_WORDLIST='/opt/wordlists/dns/alts.txt'
GITHUB_TOKENS_FILE='/mnt/efs/github.tokens'

potential_subs_file=`mktemp`
valid_subs_file=`mktemp`
osint_domains=`mktemp`
amass_temp=`mktemp`
trap "rm $potential_subs_file $valid_subs_file $amass_temp $osint_domains" EXIT 

passive_subdomain_enum() {
    amass enum -d $domain -nolocaldb -nocolor -passive -silent -json $amass_temp

	cat $amass_temp \
		| jq -r .name \
		| tee $potential_subs_file \
		| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"amass"}}'

	if [ -f $GITHUB_TOKENS_FILE ]
	then 
		github-subdomains -raw -d $domain -t $GITHUB_TOKENS_FILE \
			| anew $potential_subs_file \
			| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"github-subdomains"}}'
	fi
}

generate_brute_gueses() {
    cat $dict | xargs -I _ echo _.$domain
}

resolve_domains() {
    puredns resolve -q $potential_subs_file -r $DNS_RESOLVERS_FILE
}

generate_wordlist_alts() {
	temp=`mktemp`

	cat $valid_subs_file >$temp
	cat $osint_domains | jq -r .Asset | anew $temp >/dev/null

	altdns -i $temp -w $ALT_WORDLIST -o $potential_subs_file >/dev/null

	rm $temp
}

generate_ai_learned_alts() {
	cd /opt/tools/regulator
	python3 main.py -t $domain -f $valid_subs_file -o $potential_subs_file >/dev/null

	cd - >/dev/null
}

passive_subdomain_enum | tee $osint_domains

generate_brute_gueses | anew $potential_subs_file > /dev/null

resolve_domains \
	| tee $valid_subs_file \
	| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"dictionary_brute"}}'

generate_wordlist_alts 

resolve_domains \
	| anew $valid_subs_file \
	| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"altdns"}}'

generate_ai_learned_alts

resolve_domains \
	| anew $valid_subs_file \
	| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"regulator"}}'
