#!/bin/bash

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

sudo rm data/pwntainer.* 2>/dev/null
sudo rm -rf data/jobs 2>/dev/null