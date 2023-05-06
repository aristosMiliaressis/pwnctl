#!/bin/bash

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

adminPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/admin_password | jq -r .SecretString)

accessToken=$(curl -s -d '{"username":"admin","password":"'$adminPassword'"}' -H 'Content-Type: application/json' "${functionUrl}auth/grant" | jq -r .AccessToken)

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

# deletes all operations & EventBridge schedules
python3 -m awscurl --service lambda -X DELETE "${functionUrl}ops"

cd infra
terraform destroy
cd -
