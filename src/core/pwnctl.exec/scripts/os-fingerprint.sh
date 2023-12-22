#!/bin/bash
set -eux

ip=$1

params=()
[[ $ip =~ .*":".* ]] && params+=(-6)

os=$(nmap -Pn "${params[@]}" -O $ip | grep Running | cut -d ':' -f 2-)

[[ ! -z $os ]] && echo '{"Asset":"'$ip'","Tags":{"os":"'$os'"}}'
