#!/bin/bash

# TODO: solve fresh dns resolvers issue

domain=$1
dict='/opt/resources/wordlists/dns/top20000.txt'
DNS_RESOLVERS_FILE='/opt/resources/wordlists/dns/resolvers_top25.txt'

potential_subs_file=`mktemp`
valid_subs_file=`mktemp`
trusted_resolvers=`mktemp`

osint_subs() {
	temp=`mktemp`
	amass_temp=`mktemp`

    amass enum -d $domain -nolocaldb -nocolor -passive -silent -json $amass_temp
	cat $amass_temp | jq -r .name > $potential_subs_file

	resolve_domains > $temp
	
	cat $potential_subs_file | anew $temp | xargs -I _ printf "{\"asset\":\"_\", \"tags\"{\"Unresolvable\":true\"}}\n"
	rm $amass_temp
	rm $temp
}

generate_brute_gueses() {
    cat $dict | xargs -I _ echo _.$domain
}

resolve_domains() {
    puredns resolve -q $potential_subs_file -r $DNS_RESOLVERS_FILE
}

generate_alterations() {
	dnsgen -f $valid_subs_file | sort -u
}

osint_subs

generate_brute_gueses | anew $potential_subs_file > /dev/null

resolve_domains > $valid_subs_file

generate_alterations > $potential_subs_file

resolve_domains | anew $valid_subs_file

dig-deep.sh $valid_subs_file | sort -u 2>/dev/null

cat $valid_subs_file | sort -u
rm $potential_subs_file
rm $valid_subs_file
rm $trusted_resolvers

