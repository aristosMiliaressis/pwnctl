#!/bin/bash

domain=$1

#TODO: NSEC3, HTTPS,SRVB,CAA,SRV
RECORD_TYPES=(NS MX TXT NSEC SRV HTTPS SVCB HINFO CAA DS)
RESOLVERS_FILE='/opt/wordlists/dns/trusted-resolvers.txt'

domainfile=$1

enum_records() {
    echo $domain \
    | zdns $1 --name-servers @$RESOLVERS_FILE --result-verbosity short \
    | jq -c ".data.answers[] | select( .type == \"$1\" )" 2>/dev/null \
    | jq -r ' "\(.name) IN \(.type) \(.answer)"' 2>/dev/null \
    | grep -v null \
    | sort -u
}

for record in "${RECORD_TYPES[@]}"
do
    enum_records $record
done