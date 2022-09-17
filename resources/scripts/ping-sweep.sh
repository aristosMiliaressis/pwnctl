#!/bin/bash

rand=`mktemp`
cidr=$1

nmap -sn $cidr -oG $rand >/dev/null

cat $rand | grep 'Status: Up' | cut -f 2 -d ' '

rm $rand