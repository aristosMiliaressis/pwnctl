#!/bin/bash

input=$1

scan_svc() {
    for template in $(echo $1 | jq -c -r .Templates[])
    do
        expr='^\[(\S+)\] \[(\S+)\].*';
        
        nuclei -nc -silent -t "/opt/nuclei_templates/$template" -u $input \
            | while read output; do \
                [[ $output =~ $expr ]] && echo "{\"Asset\":\"$input\", \"Tags\":{\"${BASH_REMATCH[2]}\":\"${BASH_REMATCH[1]}\"}}"; \
            done
    done
}

while read service; 
do 
    for scope in $(echo $service | jq -c .Scope)
    do
        scopeType=$(echo $scope | jq -r .Type)
        if [[ $scopeType == "DomainRegex" ]]
        then
            [[ $(echo $input | unfurl domains) =~ $(echo $scope | jq -r .Pattern) ]] && scan_svc "$service"
        fi
    done
done <<< $(cat /opt/wordlists/cloud-services.json | jq -c '.[]')
