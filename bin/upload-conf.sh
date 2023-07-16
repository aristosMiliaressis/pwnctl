#!/bin/bash

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

adminPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/admin_password | jq -r .SecretString)

accessToken=$(curl -s -d '{"username":"admin","password":"'$adminPassword'"}' -H 'Content-Type: application/json' "${functionUrl}auth/grant" | jq -r .accessToken)

uploadDirectory() {
     srcDir=$1
     dstDir=""
     if [ $# -gt 1 ];
     then
          dstDir=$2;
          echo "Creating $dstDir"
          curl -X PUT -H "Authorization: Bearer $accessToken" ${functionUrl}fs/create?path=$dstDir 2>/dev/null
     fi

     for file in $srcDir/*;
     do
          if [ -f $file ];
          then
               echo "Uploading $file"
               curl -X PUT ${functionUrl}fs/upload?path=$dstDir${file#"$srcDir"} \
                    -H 'Content-Type: text/plain' --data-binary @$file -H "Authorization: Bearer $accessToken"
          fi
     done
}

./src/core/pwnctl.exec/scripts/get-psl.sh deployment/
cp src/core/pwnctl.exec/wordlists/cloud-services.json deployment/ 2>/dev/null
curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > deployment/resolvers.txt
curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers-trusted.txt | tr -d '[\t ]' > deployment/trusted-resolvers.txt

uploadDirectory ./deployment
uploadDirectory ./src/core/pwnctl.infra/Persistence/seed /seed

curl -X POST -H "Authorization: Bearer $accessToken" ${functionUrl}db/seed 2>/dev/null
