#!/bin/bash

vpc_id=$(terraform -chdir=infra/modules/ci output -raw vpc_id)
public_subnet_a=$(terraform -chdir=infra/modules/ci output -raw public_subnet_a)
public_subnet_b=$(terraform -chdir=infra/modules/ci output -raw public_subnet_b)
private_subnet_a=$(terraform -chdir=infra/modules/ci output -raw private_subnet_a)
private_subnet_b=$(terraform -chdir=infra/modules/ci output -raw private_subnet_b)

terraform -chdir=infra/modules/rds apply -auto-approve -var="vpc_id=$vpc_id" \
    -var="public_subnet_a=$public_subnet_a" -var="public_subnet_b=$public_subnet_b" \
    -var="private_subnet_a=$private_subnet_a" -var="private_subnet_b=$private_subnet_b"

db_host=$(terraform -chdir=infra/modules/rds output -raw db_host)
db_name=$(terraform -chdir=infra/modules/rds output -raw db_name)
db_user=$(terraform -chdir=infra/modules/rds output -raw db_user)

terraform -chdir=infra apply -auto-approve -var="vpc_id=$vpc_id" \
    -var="db_host=$db_host" -var="db_name=$db_name" -var="db_user=$db_user" \
    -var="public_subnet_a=$public_subnet_a" -var="public_subnet_b=$public_subnet_b" \
    -var="private_subnet_a=$private_subnet_a" -var="private_subnet_b=$private_subnet_b"

./bin/upload-conf.sh