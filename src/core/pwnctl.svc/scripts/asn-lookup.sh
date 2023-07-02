#!/bin/bash

ip=$1

asnmap -silent -j -ip $ip \
    | jq -c '. as $base | .as_range[] | { asset: ., tags:{ as_number:$base.as_number, as_name:$base.as_name, as_country:$base.as_country }}'