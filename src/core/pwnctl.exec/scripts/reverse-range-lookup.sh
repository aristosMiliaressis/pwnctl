#!/bin/bash
set -eux

cidr=$1

RESOLVERS_FILE='/opt/wordlists/dns/resolvers-best.txt'

mapcidr -silent -cidr $cidr \
    | zdns PTR --name-servers @$RESOLVERS_FILE --result-verbosity short \
    | jq -r '.data | select(.answers != null ) |.answers[] | select( .type == "PTR" ) | "\(.name) IN \(.type) \(.answer)"' \
    | grep -v null \
    | sort -u