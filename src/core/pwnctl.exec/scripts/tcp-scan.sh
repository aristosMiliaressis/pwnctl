#!/bin/bash
set -eu

ip=$1

naabu -silent -p 1-65535 -host $ip
