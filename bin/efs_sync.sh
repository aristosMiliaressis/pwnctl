#!/bin/bash

jumpbox_ip=$(terraform -chdir=infra/modules/jumpbox output -raw jumpbox_ip)
ssh_key='infra/modules/jumpbox/pwnctl_jumpbox.pem'

rsync -e "ssh -i $ssh_key" -azP ec2-user@$jumpbox_ip:/mnt/efs .