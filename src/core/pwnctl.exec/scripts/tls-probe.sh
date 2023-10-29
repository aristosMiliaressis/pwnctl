#!/bin/bash

addr=$1

tls_enum() {
    cert=$(tlsx -silent -u $addr -j -jarm -cert)

    echo $cert | jq -r .subject_an[] | sed 's/\*\.//' | sort -u
    echo $cert | jq -c "{Asset:\"$addr\",Tags:{ wildcard_cert: .wildcard_certificate|tostring, tls_version,dn:.subject_dn,san:.subject_an|@csv,jarm:.jarm_hash,expired,revoked,mismatched,\"self-signed\"}}" | sed 's/"\:"null",/":"",/'
}

tlsx -silent -u $addr -tps | grep -q '[success]' && tls_enum
