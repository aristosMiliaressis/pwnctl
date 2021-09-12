#!/bin/bash

datasette serve /opt/pwntainer/pwntainer.db -h 127.0.0.1 -p 8000 &>/dev/null &

# Todo Make this a cron job
/opt/recon_scripts/validatedns &>/dev/null &

while true; do sleep 1000; done