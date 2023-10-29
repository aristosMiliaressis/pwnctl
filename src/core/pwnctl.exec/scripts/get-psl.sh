#!/bin/bash

path=$1

curl -s https://publicsuffix.org/list/public_suffix_list.dat \
    | grep -Ev '\*|!|//' \
    | sed '/^[[:space:]]*$/d' > $path/public_suffix_list.dat
