#!/bin/bash

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

adminPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/admin_password | jq -r .SecretString)

accessToken=$(curl -s -d '{"username":"admin","password":"'$adminPassword'"}' -H 'Content-Type: application/json' "${functionUrl}auth/grant" | jq -r .accessToken)

# deletes all operations & EventBridge schedules
curl -H "Authorization: Bearer $accessToken" -X DELETE "${functionUrl}ops"

terraform -chdir=infra destroy
