#!/bin/bash
set -eu

srv=$1
ip=$(echo $srv | cut -d : -f 2 | tr -d '/')
port=$(echo $srv | cut -d : -f 3)

temp=`mktemp | sed 's,/tmp/,,g'`;
touch $temp
trap "rm $temp" EXIT

nmap -Pn -p $port -sV --script-args http.useragent="Mozilla/9.1" -oG $temp $ip >/dev/null;

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
		echo '{"asset":"'$srv'","tags":{"Protocol":"'$protocol'", "Version":"'$version'"}}'; \
	done
