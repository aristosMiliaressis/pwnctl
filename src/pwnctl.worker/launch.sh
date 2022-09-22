#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

if test -f "/opt/amass.ini"; 
then
    cp "/opt/amass.ini" ./data/amass.ini
fi

if test -f "/opt/provider-config.yaml"; 
then
    cp "/opt/provider-config.yaml" ./data/provider-config.yaml
fi

docker stop pwntainer 2>/dev/null
docker rm pwntainer 2>/dev/null

rm -rf data/queue 2>/dev/null
rm -rf data/pwntainer.* 2>/dev/null
rm -rf data/pwnctl.log 2>/dev/null

docker-compose pull
docker-compose up -d

docker exec -it pwntainer bash