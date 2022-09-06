#!/bin/bash

domain=$1
dict='/opt/resources/wordlists/dns/top20000.txt'
DNS_RESOLVERS_FILE='/opt/resources/wordlists/dns/resolvers_top25.txt'

potential_subs_file='potential_subs_file.log'
valid_subs_file='valid_subs_file.log'
trusted_resolvers='trusted_resolvers.log'

dig @9.9.9.9 $domain NS +short > $trusted_resolvers # TODO: error handle this

# resolve amass domains & add tag for unresolvable
# fix puredns & solve fresh dns resolvers issue

osint_subs() {
	temp='temp.log'

    amass enum -d $domain -nolocaldb -nocolor -passive | tee $potential_subs_file
	puredns resolve -q $potential_subs_file --resolvers-trusted $trusted_resolvers -r $DNS_RESOLVERS_FILE | tee $temp > $valid_subs_file
	
	cat $potential_subs_file | anew $temp | xargs -I _ printf "_${PWNCTL_DELIMITER}Unresolvable:true\n"
	rm $temp
}

generate_brute_gueses() {
    cat $dict | xargs -I _ echo _.$domain
}

resolve_domains() {
    puredns resolve -q $potential_subs_file --resolvers-trusted $trusted_resolvers -r $DNS_RESOLVERS_FILE \
		--write valid_domains.txt \
		--write-wildcards wildcards.txt \
		--write-massdns massdns.txt
}

generate_alterations() {
	dnsgen -f $valid_subs_file | sort -u
}

osint_subs > $potential_subs_file > /dev/null 2>&1

generate_brute_gueses | anew $potential_subs_file > /dev/null

resolve_domains > $valid_subs_file

generate_alterations > $potential_subs_file

resolve_domains | anew $valid_subs_file

rm $potential_subs_file
cat $valid_subs_file
rm $valid_subs_file

#dig_deep_zdns
