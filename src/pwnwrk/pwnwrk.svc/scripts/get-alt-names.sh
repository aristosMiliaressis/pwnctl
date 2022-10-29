#!/bin/bash

tlsx -u $1 -san -cn -silent -resp-only -p 443,8443,465,587 \
    | grep -v ' ' \
    | grep '\.' \
    | sort -u
