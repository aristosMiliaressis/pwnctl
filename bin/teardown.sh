#!/bin/bash

aws events remove-targets --rule all-tasks-completed --ids $(aws events list-targets-by-rule --rule all-tasks-completed | jq -r '.Targets[].Id')

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

if [ ! -z $functionUrl ]
then
    adminPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/admin_password | jq -r .SecretString)

    accessToken=$(curl -s -d '{"username":"admin","password":"'$adminPassword'"}' -H 'Content-Type: application/json' "${functionUrl}auth/grant" | jq -r .accessToken)

    # deletes all operations & EventBridge schedules
    curl -H "Authorization: Bearer $accessToken" -X DELETE "${functionUrl}ops"
fi

terraform -chdir=infra destroy
