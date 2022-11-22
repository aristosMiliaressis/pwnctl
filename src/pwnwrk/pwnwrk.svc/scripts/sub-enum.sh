#!/bin/bash

domain=$1
dict='/opt/wordlists/best-dns-wordlist.txt'
DNS_RESOLVERS_FILE='/opt/wordlists/dns/resolvers_top25.txt'

potential_subs_file=`mktemp`
valid_subs_file=`mktemp`
amass_temp=`mktemp`
echo $domain > $valid_subs_file

passive_subdomain_enum() {
    amass enum -d $domain -nolocaldb -nocolor -passive -silent -json $amass_temp

	cat $amass_temp | jq -r .name
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

passive_subdomain_enum | tee $potential_subs_file

generate_brute_gueses | anew $potential_subs_file > /dev/null

resolve_domains >> $valid_subs_file

generate_wordlist_alts 

resolve_domains | anew $valid_subs_file

generate_ai_learned_alts

resolve_domains | anew $valid_subs_file

dig-deep.sh $valid_subs_file | sort -u 2>/dev/null
cat $valid_subs_file | sort -u

rm $potential_subs_file
rm $valid_subs_file
rm $amass_temp
