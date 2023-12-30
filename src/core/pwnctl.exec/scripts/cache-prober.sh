#!/bin/bash
set -eu

url=$1

! (skiphost -u "$url" | jq -r .looks_good) && echo "skipped host" 1>&2 && exit 0

cache-prober --parallel 8 --rps 16 --max-time 60m -u "$url" \
    | jq -c 'select( .Type == "vuln" ) as $vuln | {asset:"'$url'", tags:{"cache-prober-'$(head -c 18 /dev/random | base32 | tr -d '\n')'":(.|tostring)}}'
