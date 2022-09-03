#!/bin/bash

domain=$1
dict='./recon_scripts/top20000.txt'
DNS_RESOLVERS_FILE='./recon_scripts/resolvers_top25.txt'

potential_subs_file=`mktemp`
valid_subs_file=`mktemp`

osint_subs() {
    amass enum -rf $DNS_RESOLVERS_FILE -d $domain -nolocaldb -nocolor -passive -silent | tee amass.log

    #dnsgrep -f /opt/dnsgrep/fdns_a.sort.txt -i ".$domain" \
	#    | tr ',' '\n' \
	#    | tr ' ' '\n' \
	#    | grep "$domain" \
	#    | sort -u
}

generate_brute_gueses() {
    cat $dict | xargs -I _ echo _.$domain
}

resolve_domains() {
    puredns resolve -q $potential_subs_file -r $DNS_RESOLVERS_FILE | tee puredns.log
}

generate_alterations() {
	dnsgen -f $valid_subs_file
}

osint_subs > $potential_subs_file

generate_brute_gueses | anew $potential_subs_file >/dev/null 2>&1

resolve_domains > $valid_subs_file

generate_alterations > $potential_subs_file

resolve_domains | anew $valid_subs_file

rm $potential_subs_file
cat $valid_subs_file
rm $valid_subs_file

#dig_deep_zdns
