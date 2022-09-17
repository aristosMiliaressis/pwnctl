#!/bin/bash

if ! test -f "/opt/wordlists/dns/public_suffix_list.dat"; then
    get-psl.sh
fi

echo 'get-valid-resolvers.sh' | job-queue.sh -w 1 -q "$PWNCTL_INSTALL_PATH/jobs"

while true; do sleep 10000; done