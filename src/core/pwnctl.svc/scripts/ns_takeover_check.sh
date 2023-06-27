#!/bin/bash

domain=$1

dnstake --silent -t $domain | jq -c --raw-input '. | {Asset:(.),Tags:{"ns-takeover":"true"}}'