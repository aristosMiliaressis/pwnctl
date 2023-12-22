#!/bin/bash
set -eux

emailfinder -d $1 \
    | grep 'Total emails' -A 1000 \
    | tail -n +3 \
    | grep -v ':(' || exit 0