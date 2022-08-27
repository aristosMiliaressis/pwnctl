#!/bin/bash

mkdir "$INSTALL_PATH/dns/" 2>/dev/null

wget https://publicsuffix.org/list/public_suffix_list.dat -P "$INSTALL_PATH/dns/"

cat "$INSTALL_PATH/dns/public_suffix_list.dat" | grep -v "//" | sed '/^[[:space:]]*$/d' > tmp.dat
cat tmp.dat > "$INSTALL_PATH/dns/public_suffix_list.dat"
rm tmp.dat