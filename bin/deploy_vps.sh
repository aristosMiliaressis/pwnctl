#!/bin/bash
set -e

dotnet publish src/pwnctl.cli/pwnctl.cli.csproj -c Release

cd infra/ansible/

password=$(aws secretsmanager get-secret-value --secret-id /aws/secret/pwnctl/Db/Password | jq -r .SecretString)
if [[ -z $password ]]
then
    password=$(tr -dc 'A-Za-z0-9' < /dev/urandom | head -c 16; echo)
    aws secretsmanager create-secret --name /aws/secret/pwnctl/Db/Password --secret-string $password --no-cli-pager
fi

ansible-playbook -i hosts.ini -k -u root install_postgres.yml --extra-vars "db_pass=$password"

ansible-playbook -i hosts.ini -k -u root install_cli.yml

cd -

vpc_id=$(terraform -chdir=infra/modules/ci output -raw vpc_id)
public_subnet_a=$(terraform -chdir=infra/modules/ci output -raw public_subnet_a)
public_subnet_b=$(terraform -chdir=infra/modules/ci output -raw public_subnet_b)
private_subnet_a=$(terraform -chdir=infra/modules/ci output -raw private_subnet_a)
private_subnet_b=$(terraform -chdir=infra/modules/ci output -raw private_subnet_b)

db_host=$(cat infra/ansible/hosts.ini | tail -n 1)

terraform -chdir=infra apply -auto-approve -var="vpc_id=$vpc_id" \
    -var="db_host=$db_host" -var="db_name=pwnctl" -var="db_user=pwnadmin" \
    -var="public_subnet_a=$public_subnet_a" -var="public_subnet_b=$public_subnet_b" \
    -var="private_subnet_a=$private_subnet_a" -var="private_subnet_b=$private_subnet_b"

./bin/upload-conf.sh
