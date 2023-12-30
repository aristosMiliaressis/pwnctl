#!/bin/bash
set -eu

ip=$1

whois_info=$(whois -h whois.cymru.com -v "$ip" 2>/dev/null | grep -v Warning | tail -n 1)
asn=$(echo "$whois_info" | cut -d '|' -f 1 | xargs)
cidr=$(echo "$whois_info" | cut -d '|' -f 3 | xargs)
country=$(echo "$whois_info" | cut -d '|' -f 4 | xargs)
date=$(echo "$whois_info" | cut -d '|' -f 6 | xargs)
asname=$(echo "$whois_info" | cut -d '|' -f 7 | xargs)

if [[ $cidr != "NA" ]]
then
    echo '{"asset":"'$cidr'", "tags":{"asn":"'$asn'", "asdesc":"'$asname'", "country":"'$country'", "date":"'$date'"}}'
fi