#!/bin/bash

ip=$1

os=$(nmap -Pn -O $ip | grep Running | cut -d ':' -f 2-)

[[ ! -z $os ]] && echo '{"Asset":"'$ip'","Tags":{"os":"'$os'"}}'
