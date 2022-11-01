#!/bin/bash

apiKey=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/Api/ApiKey | jq -r .SecretString)
functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

uploadDirectory() {
     srcDir=$1
     dstDir=""
     if [ $# -gt 1 ]; 
     then 
          dstDir=$2; 
          echo "Creating $dstDir"
          curl -XPUT ${functionUrl}fs/create?path=$dstDir \
                    -H "X-Api-Key: $apiKey"
     fi

     for file in $srcDir/*; 
     do 
          if [ -f $file ];
          then
               echo "Uploading $file"
               curl -XPUT ${functionUrl}fs/upload?path=$dstDir${file#"$srcDir"} \
                    -H "X-Api-Key: $apiKey" \
                    -H 'Content-Type: text/plain' \
                    --data-binary @$file
          fi          
     done
}

uploadDirectory ./deployment
uploadDirectory ./src/core/pwnwrk.infra/Persistence/seed /seed
