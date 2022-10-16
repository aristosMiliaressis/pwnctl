#!/bin/bash

apiKey=$1
functionName=$(aws lambda list-functions | jq -r '.Functions[] | select( .FunctionName  | startswith("PwnctlCdkStack-pwnctlapi")) | .FunctionName')
functionUrl=$(aws lambda get-function-url-config --function-name $functionName | jq -r .FunctionUrl)

uploadDirectory() {
     dir=$1
     for file in $dir/*; 
     do 
          if [ -d $file ];
          then
               uploadDirectory $file
          else
               echo "Uploading $file"
               curl -XPUT ${functionUrl}fs/upload?path=/${file#"$dir"} \
                    -H "X-Api-Key: $apiKey" \
                    -H 'Content-Type: text/plain' \
                    --data-binary @$file
          fi          
     done
}

uploadDirectory ./deployment