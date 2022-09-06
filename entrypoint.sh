#!/bin/bash

export PWNCTL_DELIMITER=`printf "\x1E"`

if ! test -f "$PWNCTL_INSTALL_PATH/dns/public_suffix_list.dat"; then
    get-psl.sh
fi

while true; do sleep 10000; done