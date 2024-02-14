#!/bin/bash

regexes=(
    "[\!(,%]" # Ignore noisy characters
    "^[#&?;]" # Ignore lines that start with path terminators
    "^[\!\@\#\$%^&*\(\)\-\+\=\_\[}{\\\|\;\:\"\,><\./\?\`~]*$" # Ignore lines with only special characters
    ".{100,}" # Ignore lines with more than 100 characters (overly specific)
    "[0-9]{4,}" # Ignore lines with 4 or more consecutive digits (likely an id)
    "[0-9]{3,}$" # Ignore lines where the last 3 or more characters are digits (likely an id)
    "[a-z0-9]{32}" # Likely MD5 hash or similar
    "[0-9]+[A-Z0-9]{5,}" # Number followed by 5 or more numbers and uppercase letters (almost all noise)
    "\/.*\/.*\/.*\/.*\/.*\/.*\/" # Ignore lines more than 6 directories deep (overly specific)
    "\w{8}-\w{4}-\w{4}-\w{4}-\w{12}" # Ignore UUIDs
    "[0-9]+[a-zA-Z]+[0-9]+[a-zA-Z]+[0-9]+" # Ignore multiple numbers and letters mixed together (likley noise)
    "\.(png|jpg|jpeg|gif|svg|bmp|ttf|avif|wav|mp4|aac|ajax|css|all|)$" # Ignore low value filetypes
    "^http" # Ignore web addresses
)

wordlist=$1
temp=`mktemp`
trap "rm $temp" EXIT

cmd="cat $wordlist"
for regex in "${regexes[@]}"; do
    cmd="$cmd | grep -avE '${regex}'"
done
eval "${cmd} > $temp"
duplicut $temp -o $wordlist &>/dev/null
