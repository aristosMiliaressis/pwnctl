#!/bin/bash

addr=$1

tls_enum() {
    # TODO: capture non dns Subject Alt Names
    tlsx -silent -u $addr -san -cn -resp-only | grep -v ' ' | grep '\.' | sort -u

    resp=$(tlsx -silent -j -jarm -ve -so -ex -ss -mm -re -u $addr)

    tmp="{\"Asset\":\"$addr\",\"Tags\":{\"encrypted\":\"true\", \"jarm\":\"$(jq -r .jarm_hash <<< $resp)\", \"tls_versions\":\"$(jq -r '.version_enum | @csv' <<< $resp)\"}}"
    tmp=$(jq -c --argjson tmp "$tmp" 'if .expired then $tmp | .Tags.expired="true" else $tmp end' <<< $resp)
    tmp=$(jq -c --argjson tmp "$tmp" 'if .revoked then $tmp | .Tags.revoked="true" else $tmp end' <<< $resp)
    tmp=$(jq -c --argjson tmp "$tmp" 'if .mismatched then $tmp | .Tags.mismatched="true" else $tmp end' <<< $resp)
    tmp=$(jq -c --argjson tmp "$tmp" 'if ."self-signed" then $tmp | .Tags."self-signed"="true" else $tmp end' <<< $resp)

    #TODO: sslscan
	#TODO: tlsx -sni '' # SSRF Injection test

    echo $tmp
}

tlsx -silent -u $addr -tps | grep -q '[success]' && tls_enum 
