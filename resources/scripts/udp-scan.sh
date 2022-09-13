#!/bin/bash

ip=$1
temp=`mktemp`;

nmap -sU -sV --version-intensity 0 -F -n $ip -oG $temp >/dev/null;

cat $temp \
	| sed 's/Ports: /\n/g' \
	| sed 's/, /\n/g' \
	| grep 'open/udp' \
	| cut -d '/' -f 1,5,7 \
	| while read line; \
	do \
		port=`echo $line | cut -d '/' -f 1`; \
		protocol=`echo $line | cut -d '/' -f 2`; \
		version=`echo $line | cut -d '/' -f 3`; \
		printf "{\"asset\":\"$ip:U$port\",\"tags\":{\"Protocol\":\"$protocol\", \"Version\":\"$version\"}}\n"; \
	done

rm $temp
