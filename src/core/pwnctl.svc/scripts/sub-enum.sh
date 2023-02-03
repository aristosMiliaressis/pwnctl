#!/bin/bash

# TODO: add subfinder & assetfinder & finddomain
# TODO: troubleshoot dictionary_brute/altdns/regulator

domain=$1
dict='/opt/wordlists/subdomains-top-20000.txt'
DNS_RESOLVERS_FILE='/opt/wordlists/dns/resolvers_top25.txt' # https://github.com/blechschmidt/massdns/blob/master/lists/resolvers.txt
GITHUB_TOKENS_FILE='/mnt/efs/github.tokens'

potential_subs_file=`mktemp`
valid_subs_file=`mktemp`
amass_temp=`mktemp`
trap "rm $potential_subs_file $valid_subs_file $amass_temp" EXIT 

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
	altdns -i $valid_subs_file -w /opt/wordlists/dns/alts.txt -o $potential_subs_file
}

generate_ai_learned_alts() {
	regulator_rules=`mktemp`

	cd /opt/tools/regulator
	python3 main.py $domain $valid_subs_file $regulator_rules
	./make_brute_list.sh $regulator_rules $potential_subs_file
	
	rm $regulator_rules
	cd - >/dev/null
}

passive_subdomain_enum

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

echo $domain >> $valid_subs_file
dig-deep.sh $valid_subs_file \
	| sort -u 2>/dev/null \
	| xargs -I {} -n1 echo '{"Asset":"{}", "Tags":{"tool":"zdns"}}'
