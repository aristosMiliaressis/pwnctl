#!/bin/bash

mkdir "$PWNCTL_INSTALL_PATH/dns/" 2>/dev/null

wget https://publicsuffix.org/list/public_suffix_list.dat -P "$PWNCTL_INSTALL_PATH/dns/"

cat "$PWNCTL_INSTALL_PATH/dns/public_suffix_list.dat" | grep -v "//" | sed '/^[[:space:]]*$/d' > tmp.dat
cat tmp.dat > "$PWNCTL_INSTALL_PATH/dns/public_suffix_list.dat"
rm tmp.dat