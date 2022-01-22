#!/bin/bash

datasette serve /opt/pwntainer/pwntainer.db -h 127.0.0.1 -p 8000 &>/dev/null &

while true; do sleep 10000; done