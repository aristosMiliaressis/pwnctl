#!/bin/bash

path=$1
temp=`mktemp`

rm $path/public_suffix_list.dat 2>/dev/null

curl -s https://publicsuffix.org/list/public_suffix_list.dat > $path/public_suffix_list.dat &>/dev/null

cat "$path/public_suffix_list.dat" | grep -Ev '\*|!|//' | sed '/^[[:space:]]*$/d' > $temp

cat $temp > "$path/public_suffix_list.dat"
rm $temp