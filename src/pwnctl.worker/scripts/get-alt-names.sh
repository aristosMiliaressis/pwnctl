#!/bin/bash

echo $1 | cero -d -p 443,465,587,993,995,8443,8880,2525 | sort -u