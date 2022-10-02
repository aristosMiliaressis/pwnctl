#!/bin/bash

region='us-east-1'
sftp_user='sftp-user'
server_id=$(aws transfer list-servers --region $region | jq -r '.Servers[] | select( .Domain == "EFS" and .EndpointType == "PUBLIC" ) | .ServerId')
filesystem_id=$(aws efs describe-file-systems --region $region | jq -r '.FileSystems[] | select( .Name == "pwnwrk-fs2" ) | .FileSystemId')

aws transfer start-server --server-id $server_id --region $region

while [ `aws transfer describe-server --server-id $server_id --region $region | jq -r '.Server | select( .Domain == "EFS" and .EndpointType == "PUBLIC" ) | .State'` != "ONLINE" ]
do
    sleep 10
done

#scp config.ini $sftp_user@$server_id.server.transfer.$region.amazonaws.com:/$filesystem_id/
#scp amass.ini $sftp_user@$server_id.server.transfer.$region.amazonaws.com:/$filesystem_id/
#scp provider-config.yaml $sftp_user@$server_id.server.transfer.$region.amazonaws.com:/$filesystem_id/
scp data/entrypoint_hook.sh $sftp_user@$server_id.server.transfer.$region.amazonaws.com:/$filesystem_id/
scp -r data/seed/ $sftp_user@$server_id.server.transfer.$region.amazonaws.com:/$filesystem_id/

aws transfer stop-server --server-id $server_id --region $region
