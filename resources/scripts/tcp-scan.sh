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
		echo "$ip:$port [[Protocol:$protocol]$([[ ! -z "$version" ]] && echo "[Version:$version]")]"; \
	done

rm $temp