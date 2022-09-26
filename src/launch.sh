#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

if test -f "/opt/amass.ini"; 
then
    cp "/opt/amass.ini" ./data/amass.ini
fi

if test -f "/opt/aws.config"; 
then
    cp "/opt/aws.config" ./data/aws.config
fi

if test -f "/opt/aws.credentials"; 
then
    cp "/opt/aws.credentials" ./data/aws.credentials
fi

if test -f "/opt/provider-config.yaml"; 
then
    cp "/opt/provider-config.yaml" ./data/provider-config.yaml
fi

docker stop pwnwrk 2>/dev/null
docker rm pwnwrk 2>/dev/null

rm -rf data/queue 2>/dev/null
rm -rf data/pwnctl.* 2>/dev/null
rm -rf data/pwnctl.log 2>/dev/null

docker-compose pull
docker-compose up -d

docker exec -it pwnwrk bash