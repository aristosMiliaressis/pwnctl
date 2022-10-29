#!/bin/bash

apiKey=$(aws secretsmanager get-secret-value --secret-id pwnctl-Api-ApiKey | jq -r .SecretString)
functionName=$(aws lambda list-functions | jq -r '.Functions[] | select( .FunctionName  | startswith("pwnctl-pwnctlapi")) | .FunctionName')
functionUrl=$(aws lambda get-function-url-config --function-name $functionName | jq -r .FunctionUrl)

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
