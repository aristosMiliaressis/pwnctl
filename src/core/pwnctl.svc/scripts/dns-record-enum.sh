#!/bin/bash

domain=$1

RECORD_TYPES=(ANY A AAAA NS CNAME MX TXT SRV HINFO NSEC HTTPS SVCB)
DNS_RESOLVERS_FILE='/opt/wordlists/dns/resolvers.txt'

domainfile=$1

enum_records() {
    echo $domain \
    | zdns $1 --name-servers @$DNS_RESOLVERS_FILE --result-verbosity short \
    | jq -c ".data.answers[] | select( .type == \"$1\" )" 2>/dev/null \
    | jq -r ' "\(.name) IN \(.type) \(.answer)"' 2>/dev/null \
    | grep -v null \
    | sort -u
}

for record in "${RECORD_TYPES[@]}"
do
    enum_records $record
done