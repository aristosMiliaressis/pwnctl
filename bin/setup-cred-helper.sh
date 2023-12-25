#!/bin/bash

registryId=$(aws ecr describe-registry | jq -r .registryId)
region=$(aws configure get region)

cat >~/.docker/config.json <<EOF
{
  "credHelpers": {
    "${registryId}.dkr.ecr.${region}.amazonaws.com": "ecr-login"
  }
}
EOF