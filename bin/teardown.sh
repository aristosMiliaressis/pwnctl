#!/bin/bash

functionUrl=$(aws ssm get-parameter --name /pwnctl/Api/BaseUrl | jq -r .Parameter.Value)

if [ ! -z $functionUrl ]
then
    adminPassword=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/admin_password | jq -r .SecretString)

    accessToken=$(curl -s -d '{"username":"admin","password":"'$adminPassword'"}' -H 'Content-Type: application/json' "${functionUrl}auth/grant" | jq -r .accessToken)

    # deletes all operations & EventBridge schedules
    curl -H "Authorization: Bearer $accessToken" -X DELETE "${functionUrl}ops"
fi

vpc_id=$(terraform -chdir=infra/modules/ci output -raw vpc_id)
public_subnet_a=$(terraform -chdir=infra/modules/ci output -raw public_subnet_a)
public_subnet_b=$(terraform -chdir=infra/modules/ci output -raw public_subnet_b)
private_subnet_a=$(terraform -chdir=infra/modules/ci output -raw private_subnet_a)
private_subnet_b=$(terraform -chdir=infra/modules/ci output -raw private_subnet_b)
db_host=$(terraform -chdir=infra/modules/rds output -raw db_host)
db_name=$(terraform -chdir=infra/modules/rds output -raw db_name)
db_user=$(terraform -chdir=infra/modules/rds output -raw db_user)

terraform -chdir=infra destroy -var="vpc_id=$vpc_id" \
    -var="db_host=$db_host" -var="db_name=$db_name" -var="db_user=$db_user" \
    -var="public_subnet_a=$public_subnet_a" -var="public_subnet_b=$public_subnet_b" \
    -var="private_subnet_a=$private_subnet_a" -var="private_subnet_b=$private_subnet_b"

terraform -chdir=infra/modules/rds destroy -var="vpc_id=$vpc_id" \
    -var="public_subnet_a=$public_subnet_a" -var="public_subnet_b=$public_subnet_b" \
    -var="private_subnet_a=$private_subnet_a" -var="private_subnet_b=$private_subnet_b"
