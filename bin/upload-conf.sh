#!/bin/bash

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

adminPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/admin_password | jq -r .SecretString)

accessToken=$(curl -s -d '{"username":"admin","password":"'$adminPassword'"}' -H 'Content-Type: application/json' "${functionUrl}auth/grant" | jq -r .accessToken)

uploadDirectory() {
     localDir=$1
     remoteDir="/"
     [[ $# -eq 2 ]] && remoteDir=$2

     for file in ${localDir}${remoteDir}*;
     do
          if [ -f $file ];
          then
               file=$(echo $file | sed "s,$localDir,," | sed 's,//,/,')
               echo "Uploading $file"
               curl -X PUT ${functionUrl}fs/upload?path=${file#"$localDir"} \
                    -H 'Content-Type: text/plain' --data-binary @$localDir/$file -H "Authorization: Bearer $accessToken"
          fi
     done

     for directory in ${localDir}${remoteDir}/*/;
     do
          if [[ ! -d "$directory" ]]; then continue; fi

          directory=$(echo $directory | sed "s,$localDir,," | sed 's,//,/,')
          echo "Creating $directory"
          curl -X PUT -H "Authorization: Bearer $accessToken" ${functionUrl}fs/create?path=$directory 2>/dev/null

          uploadDirectory $localDir $directory
     done
}

curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers.txt > deployment/resolvers.txt
curl -s https://raw.githubusercontent.com/trickest/resolvers/main/resolvers-trusted.txt | tr -d '[\t ]' > deployment/trusted-resolvers.txt

uploadDirectory ./deployment

curl -X POST -H "Authorization: Bearer $accessToken" ${functionUrl}db/seed 2>/dev/null
