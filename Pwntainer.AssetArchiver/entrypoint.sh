#!/bin/bash

datasette serve /opt/pwntainer/pwntainer.db -h 127.0.0.1 -p 8000 &>/dev/null &

# Todo Make this a cron job
/opt/recon_scripts/validatedns &>/dev/null &

chmod -R +x /app/workflows/
mv /app/workflows/hourly/* /etc/cron.hourly/
mv /app/workflows/daily/* /etc/cron.daily/
mv /app/workflows/weekly/* /etc/cron.weekly/
mv /app/workflows/monthly/* /etc/cron.monthly/

while true; do sleep 1000; done