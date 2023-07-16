#!/bin/bash

email=$1

curl -s "https://viewdns.info/reversewhois/?q=$email" \
	-H 'user-agent: Mozilla/5.0 (X11; Fedora; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36' \
	| htmlq  table#null tr \
	| tail -n +3 \
	| htmlq -t td \
	| grep -P '[\w-]+\.[\.\w-]+'