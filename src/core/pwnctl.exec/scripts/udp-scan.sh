#!/bin/bash
set -eu

ip=$1
temp=`mktemp`;
trap "rm $temp" EXIT

params=()
[[ $ip =~ .*":".* ]] && params+=(-6)

nmap -Pn -sU --script-args http.useragent="$(uagen)" -sV "${params[@]}" --version-intensity 3 -F -n $ip -oG $temp >/dev/null;

cat $temp \
	| sed 's/Ports: /\n/g' \
	| sed 's/, /\n/g' \
	| grep -E '(open|filtered)/udp' \
	| cut -d '/' -f 1,5,7 \
	| while read line; \
	do \
		port=`echo $line | cut -d '/' -f 1`; \
		protocol=`echo $line | cut -d '/' -f 2`; \
		version=`echo $line | cut -d '/' -f 3`; \
		echo '{"asset":"udp://'$ip:$port'","tags":{"Protocol":"'$protocol'", "Version":"'$version'"}}'; \
	done


