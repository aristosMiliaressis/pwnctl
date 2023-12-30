#!/bin/bash
set -eu

ip=$1
temp=`mktemp | sed 's,/tmp/,,g'`;
touch $temp
trap "rm $temp" EXIT

rustscan -r 1-65535 -a $ip -- -sSV --script-args http.useragent="Mozilla/9.1" -oG $temp >/dev/null;

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
		echo '{"asset":"'$ip:$port'","tags":{"Protocol":"'$protocol'", "Version":"'$version'"}}'; \
	done
