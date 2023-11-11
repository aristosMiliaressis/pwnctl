#!/bin/bash

addr=$1

tls_enum() {
    cert=$(tlsx -silent -u $addr -j -jarm -cert)

    echo $cert | jq -r .subject_an[] 2>/dev/null | sed 's/\*\.//' | sort -u
    echo $cert \
        | jq -c '. | if (.subject_an == null) then .subject_an = [] else . end' \
        | jq -c "{Asset:\"$addr\",Tags:{ wildcard_cert: .wildcard_certificate|tostring, tls_version,dn:.subject_dn,san:.subject_an|@csv,jarm:.jarm_hash,expired:.expired|tostring,revoked:.revoked|tostring,mismatched:.mismatched|tostring,\"self-signed\":.\"self-signed\"|tostring}}" \
        | sed 's/"null"/""/g'
}

tlsx -silent -u $addr -tps | grep -q '[success]' && tls_enum
