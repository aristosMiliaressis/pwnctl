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
curl -s https://raw.githubusercontent.com/proabiral/Fresh-Resolvers/master/resolvers.txt | shuf -n 25 > deployment/resolvers_top25.txt

uploadDirectory ./deployment
uploadDirectory ./src/core/pwnctl.infra/Persistence/seed /seed
