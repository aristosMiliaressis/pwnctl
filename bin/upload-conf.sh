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

./src/core/pwnctl.svc/scripts/get-psl.sh deployment/
cp src/core/pwnctl.svc/wordlists/cloud-services.json deployment/ 2>/dev/null
curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > deployment/resolvers.txt
curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers-trusted.txt > deployment/trusted-resolvers.txt

uploadDirectory ./deployment
uploadDirectory ./src/core/pwnctl.infra/Persistence/seed /seed
