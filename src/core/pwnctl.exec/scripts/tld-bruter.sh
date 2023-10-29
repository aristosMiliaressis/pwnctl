#!/bin/bash

if [ $# -lt 1 ]
then
	echo "$0 <keyword>"
fi

keyword=$1

# TODO download
PSL='deployment/public_suffix_list.dat'
RESOLVERS='/opt/wordlists/dns/trusted-resolvers.txt'

cat $PSL \
	| xargs -I {} printf "$keyword.{}\n" \
	| zdns A --name-servers @$RESOLVERS \
	| jq -r 'select( .status == "NOERROR" ) | .name' \
	| tee target_tlds.txt | httpx --silent --screenshot \
	| tee target_tld_servers.txt
