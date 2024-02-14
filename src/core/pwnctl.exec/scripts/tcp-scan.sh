#!/bin/bash
set -eu

ip=$1

naabu -silent -Pn -exclude-cdn -p 1-65535 -host $ip
