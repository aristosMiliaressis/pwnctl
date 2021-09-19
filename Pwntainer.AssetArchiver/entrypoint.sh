#!/bin/bash

cat /app/aliases.txt >> /root/.bashrc

datasette serve /opt/pwntainer/pwntainer.db -h 127.0.0.1 -p 8000 &>/dev/null &

mv /app/recon_scripts/resolvers_top25.txt /opt/dnsvalidator/
mv /app/recon_scripts/top200000.txt /opt/pwntainer/lists/
mv /app/recon_scripts/top20000.txt /opt/pwntainer/lists/
mv /app/recon_scripts/* /usr/local/bin
rm -r /app/recon_scripts/

chmod -R +x /app/workflows/
mv /app/workflows/hourly/* /etc/cron.hourly/
mv /app/workflows/daily/* /etc/cron.daily/
mv /app/workflows/weekly/* /etc/cron.weekly/
mv /app/workflows/monthly/* /etc/cron.monthly/

while true; do sleep 1000; done