#!/bin/bash

path=$1
temp=`mktemp`

rm $path/public_suffix_list.dat 2>/dev/null

wget -q https://publicsuffix.org/list/public_suffix_list.dat -P $path &>/dev/null

cat "$path/public_suffix_list.dat" | grep -Ev '\*|!|//' | sed '/^[[:space:]]*$/d' > $temp

cat $temp > "$path/public_suffix_list.dat"
rm $temp