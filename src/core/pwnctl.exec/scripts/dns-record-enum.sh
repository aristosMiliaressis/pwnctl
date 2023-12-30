#!/bin/bash
set -eu

domain=$1

RECORD_TYPES=(NS MX TXT NSEC HTTPS SVCB HINFO)
RESOLVERS_FILE='/opt/wordlists/dns/resolvers-best.txt'

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

dig +nottlid TXT _dmarc.$domain 2>/dev/null | tr '\t' ' ' | grep ' IN TXT ' || echo -n
dig +nottlid TXT _mta-sts.$domain 2>/dev/null | tr '\t' ' ' | grep ' IN TXT ' || echo -n

if [[ $2 == "1" ]]
then
    cat /opt/wordlists/dns/srv-records.txt | xargs -I {} dig +nottlid SRV {}.$domain | tr '\t' ' ' | grep ' IN SRV ' || exit 0
fi
