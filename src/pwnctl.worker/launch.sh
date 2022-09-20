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

docker build . -t pwntainer
docker stop pwntainer
docker rm pwntainer

docker-compose up -d

rm data/pwntainer.* 2>/dev/null
rm -rf data/jobs 2>/dev/null