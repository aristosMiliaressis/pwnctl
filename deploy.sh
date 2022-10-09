#!/bin.bash

apiKey=$1
functionUrl=$(aws lambda get-function-url-config --function-name PwnctlApi | jq -r .FunctionUrl)

url -XPUT ${functionUrl}fs/create?path=/seed -H "X-Api-Key: $apiKey"

curl -XPUT ${functionUrl}fs/upload?path=/entrypoint_hook.sh \
     -H "X-Api-Key: $apiKey" \
     -H 'Content-Type: text/plain' \
     --data-binary @src/pwnwrk.svc/data/entrypoint_hook.sh

curl -XPUT ${functionUrl}fs/upload?path=/seed/task-definitions.yml \
     -H "X-Api-Key: $apiKey" \
     -H 'Content-Type: text/plain' \
     --data-binary @src/pwnwrk.svc/data/seed/task-definitions.yml

curl -XPUT ${functionUrl}fs/upload?path=/seed/notification-rules.yml \
     -H "X-Api-Key: $apiKey" \
     -H 'Content-Type: text/plain' \
     --data-binary @src/pwnwrk.svc/data/seed/notification-rules.yml

# seed/target-*.json
# config.ini
# amass.ini
# aws.credentials
# aws.config
# provider-config.yaml