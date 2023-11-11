#!/bin/bash

svc=$1

echo $svc | sed 's/.*:\/\///g'| httpx -silent -csp-probe