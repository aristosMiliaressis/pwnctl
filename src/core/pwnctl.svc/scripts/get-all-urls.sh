#!/bin/bash

domain=$1

echo $domain | gau --threads 40 --timeout 20 --blacklist png,jpg,jpeg,gif,ico,svg,ttf,woff,woff2,eot,css,pdf
