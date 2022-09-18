#!/bin/bash

if test -f "/opt/amass.ini"; 
then
    cp "/opt/amass.ini" ./data/config.ini
fi

docker build . -t pwntainer
docker stop pwntainer
docker rm pwntainer

docker-compose up -d

sudo rm data/pwntainer.*
sudo rm -rf data/jobs