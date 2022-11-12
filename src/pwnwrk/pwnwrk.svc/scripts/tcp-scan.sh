#!/bin/bash

ip=$1
temp=`mktemp`;

rustscan -r 1-65535 -a $ip -- -sSV --script-args http.useragent="Mozilla/9.1 (compatible; Windows       NT 5.0 build 1420;)" -oG $temp >/dev/null;
# naabu -silent -p 0-65535 -host $ip -nmap "nmap -sSVC --script-args http.useragent='$(uagen)' -oG $temp" >/dev/null

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
