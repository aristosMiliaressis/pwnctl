#!/bin/bash

if [ $# -eq 0 ];
then
    echo "USAGE: $0 <hostfile>"
    exit 1
fi

RECORD_TYPES=(ANY A AAAA NS CNAME MX TXT SRV HTTPS HINFO NSEC)
RESOLVERS=1.1.1.1,8.8.8.8,8.8.4.4,9.9.9.9

domainfile=$1

enum_records() {
    cat $domainfile \
    | zdns $1 --name-servers $RESOLVERS --result-verbosity short \
    | jq -c ".data.answers[] | select( .type == \"$1\" )" 2>/dev/null \
    | jq -r ' "\(.name) IN \(.type) \(.answer)"' 2>/dev/null \
    | grep -v null \
    | sort -u
}

for record in "${RECORD_TYPES[@]}"
do
    enum_records $record
done