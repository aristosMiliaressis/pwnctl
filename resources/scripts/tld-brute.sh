#!/bin/bash

DNS_RESOLVERS_FILE='/opt/resources/wordlists/dns/resolvers_top25.txt'
WILDCARD_TESTS=30

#wget -O $DNS_RESOLVERS_FILE https://raw.githubusercontent.com/janmasarik/resolvers/master/resolvers.txt 2> /dev/null

filteredfile=`mktemp`

#$(python -c "import sys; from publicsuffix2 import get_tld, fetch, PublicSuffixList;psl_file = fetch();public_suffix = get_tld(sys.argv[1], psl_file);stripped_domain = sys.argv[1].replace(public_suffix, "");psl = PublicSuffixList();[print(f'{stripped_domain}{suffix}') for suffix in psl.tlds]")
filter_wildcards() {
	while read domain
	do
		ip="$(echo $domain | dnsx -silent -a -resp-only | sort)"
		tld=$(python -c "from publicsuffix2 import get_tld,fetch; psl_file = fetch();print(get_tld(\"$domain\", psl_file))")

		wildcard="*.$tld"
		if [ "$ip" != "$(echo $wildcard | dnsx -silent -a -resp-only | sort)" ]; 
		then 
			echo $domain;
		fi
	done < "${1:-/dev/stdin}"
}

tld-altgen.py $1 | grep -v -e '[*!]' \
	| dnsx -silent -r $DNS_RESOLVERS_FILE \
	| filter_wildcards > $filteredfile
	
cat $filteredfile | puredns resolve -q --wildcard-tests $WILDCARD_TESTS -r $DNS_RESOLVERS_FILE
