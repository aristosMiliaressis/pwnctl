#!/bin/bash

ip=$1
temp=`mktemp | sed 's/\/tmp\///g'`;
touch $temp

sudo naabu -silent -Pn -ec -p 1-65535 -host $ip \
 	-nmap-cli "nmap -sSV -oG ./$temp" >/dev/null

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
		printf "{\"asset\":\"$ip:$port\",\"tags\":{\"Protocol\":\"$protocol\", \"Version\":\"$version\"}}\n"; \
	done

rm $temp
