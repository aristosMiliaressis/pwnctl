#!/bin/bash
set -eux

cidr=$1

# if it contains ':' collons it is ipv6 so add -6 param
params=()
[[ $cidr =~ .*":".* ]] && params+=(-6)

nmap -n -sn -PO -PS -PE $cidr -vv "${params[@]}" 2>/dev/null \
    | grep 'Host is up' -B 1 \
    | tr '\n' ' ' \
    | sed 's/Nmap scan report/\nNmap scan report/g' \
    | cut -d ' ' -f 5,10,12 \
    | grep . \
    | jq -cnR '[inputs | split(" ") | {"Asset": .[0], "Tags":{"technique":.[1], "ttl":.[2]}}] | .[]'
