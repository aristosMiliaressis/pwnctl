#!/bin/bash

domain=$1
dict='/opt/wordlists/dns/top20000.txt'
DNS_RESOLVERS_FILE='/opt/wordlists/dns/resolvers_top25.txt'

potential_subs_file=`mktemp`
valid_subs_file=`mktemp`

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
    cat $dict | xargs -I _ echo _.$domain | anew $potential_subs_file > /dev/null
}

resolve_domains() {
    puredns resolve -q $potential_subs_file -r $DNS_RESOLVERS_FILE
}

generate_alterations() {
	dnsgen -f $valid_subs_file | sort -u > $potential_subs_file
}

echo $domain > $valid_subs_file

osint_subs

generate_brute_gueses

resolve_domains >> $valid_subs_file

generate_alterations

resolve_domains | anew $valid_subs_file

dig-deep.sh $valid_subs_file | sort -u 2>/dev/null
cat $valid_subs_file | sort -u

rm $potential_subs_file
rm $valid_subs_file
