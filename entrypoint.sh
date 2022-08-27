#!/bin/bash

if ! test -f "$INSTALL_PATH/dns/public_suffix_list.dat"; then
    /app/scripts/get_public_suffixes.sh
fi

while true; do sleep 10000; done