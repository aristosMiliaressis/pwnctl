#!/bin/bash

domain=$1

echo $domain | gau --threads 20 --timeout 25 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf \
    | grep . \
    | unfurl format '%s://%a%p%?%q'
