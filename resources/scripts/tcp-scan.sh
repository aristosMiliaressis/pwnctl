#!/bin/bash

ip=$1
temp=`mktemp`;

rustscan -r 1-65535 -a $ip -- -sSV -oG $temp >/dev/null;

cat $temp \
	| sed 's/Ports: /\n/g' \
	| sed 's/, /\n/g' \
	| grep 'open/tcp' \
	| cut -d '/' -f 1,5,7 \
	| while read line; \
	do \
		port=`echo $line | cut -d '/' -f 1`; \
		protocol=`echo $line | cut -d '/' -f 2`; \
		version=`echo $line | cut -d '/' -f 3`; \
		printf "$ip:$port${PWNCTL_TAG_DELIMITER}Protocol${PWNCTL_KV_DELIMITER}$protocol$([[ ! -z "$version" ]] && printf "${PWNCTL_TAG_DELIMITER}Version${PWNCTL_KV_DELIMITER}$version")\n"; \
	done

#printf "$ip:$port${PWNCTL_TAG_DELIMITER}Protocol${PWNCTL_KV_DELIMITER}$protocol$([[ ! -z "$version" ]] && printf "${PWNCTL_TAG_DELIMITER}Version${PWNCTL_KV_DELIMITER}$version")\n"; \
#printf "{\"asset\":\"$ip:$port\",\"tags\":{\"Protocol\":\"$protocol\", \"Version\":\"$version\"]}\n"

rm $temp
