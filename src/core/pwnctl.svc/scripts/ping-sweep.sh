#!/bin/bash

cidr=$1

sudo nmap -n -sn -PO -PE $cidr -vv \
    | grep 'Host is up' -B 1 \
    | tr '\n' ' ' \
    | sed 's/Nmap scan report/\nNmap scan report/g' \
    | cut -d ' ' -f 5,12 \
    | grep . \
    | jq -cnR '[inputs | split(" ") | {"Asset": .[0], "Tags":{"ttl": .[1]}}] | .[]'
