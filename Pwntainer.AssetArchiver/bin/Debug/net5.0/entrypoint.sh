#!/bin/bash

alias get-domains="curl -s http://localhost:8000/pwntainer/Domains.json | jq .rows[][0] | tr -d '\"'"
alias get-hosts="curl -s http://localhost:8000/pwntainer/Hosts.json | jq .rows[][0] | tr -d '\"'"
alias get-services="curl -s http://localhost:8000/pwntainer/Services.json | jq .rows[][0] | tr -d '\"'"

datasette serve /opt/pwntainer/pwntainer.db -h 127.0.0.1 -p 8000 &>/dev/null &

mv /app/resolvers_top25.txt /opt/dnsvalidator/

chmod -R +x /app/workflows/
mv /app/workflows/hourly/* /etc/cron.hourly/
mv /app/workflows/daily/* /etc/cron.daily/
mv /app/workflows/weekly/* /etc/cron.weekly/
mv /app/workflows/monthly/* /etc/cron.monthly/

while true; do sleep 1000; done