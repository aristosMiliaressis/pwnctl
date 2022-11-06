#!/bin/bash

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

uploadDirectory() {
     srcDir=$1
     dstDir=""
     if [ $# -gt 1 ]; 
     then 
          dstDir=$2; 
          echo "Creating $dstDir"
          python3 -m awscurl -X PUT --service lambda ${functionUrl}fs/create?path=$dstDir 2>/dev/null
     fi

     for file in $srcDir/*; 
     do 
          if [ -f $file ];
          then
               echo "Uploading $file"
               python3 -m awscurl -X PUT ${functionUrl}fs/upload?path=$dstDir${file#"$srcDir"} \
                    --service lambda -d @$file \
                    -H 'Content-Type: text/plain'
          fi          
     done
}

setupDb() {
     # SecretsManager is not used by the lambda cause it is in
     # a private subnet and would require a Vpc Endpoint to
     # access SecretsManager api which costs 7.20$ + tax per month
     dbHostname=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/Db | jq -r .SecretString | jq -r .host)
     dbPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/Db | jq -r .SecretString | jq -r .password)
     python3 -m awscurl -X PUT ${functionUrl}fs/upload?path=/config.ini \
                         --service lambda \
                         -H 'Content-Type: text/plain' \
                         -d "$(printf "[Db]\nHost = $dbHostname\nPassword = $dbPassword\n")"
                         


     python3 -m awscurl --service lambda -X POST ${functionUrl}db/seed
}

uploadDirectory ./deployment
uploadDirectory ./src/core/pwnwrk.infra/Persistence/seed /seed

setupDb
