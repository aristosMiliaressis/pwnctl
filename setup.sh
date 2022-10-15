#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
  exit
fi

if ! command -v aws &> /dev/null
then
  curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o awscliv2.zip
  unzip awscliv2.zip
  rm awscliv2.zip
  ./aws/install --update
  rm -rf ./aws
fi

aws configure

if ! command -v cdk &> /dev/null
then
  if ! command -v node &> /dev/null
  then
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -y nodejs
  fi

  npm install -g aws-cdk
fi

# install pwnctl cli
curl https://raw.githubusercontent.com/aristosMiliaressis/pwnctl/master/src/pwnctl.cli/install.sh | sudo bash

region=$1
connectionString=$2
accountId=$(aws sts get-caller-identity --query "Account" --output text)

# bootsrap aws env & deploy app
cdk bootstrap aws://$accountId/$region

cdk deploy PwnctlCdkStack --parameters connectionString="$connectionString"
