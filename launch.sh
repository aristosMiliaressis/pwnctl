#!/bin/bash

docker build . -t pwntainer
docker stop pwntainer
docker rm pwntainer

docker-compose up -d

