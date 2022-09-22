#!/bin/bash

if [[ $(wc -l $PWNCTL_INSTALL_PATH/queue/pending | cut -d ' ' -f 1) -eq 1 ]]
then 
    exit
fi

temp=`mktemp`

echo "SELECT Name,Value FROM Tags" | pwnctl query | jq -r 'select( .Name == "foundby" ) | .Value' | sort | uniq -c | sort  > $temp
echo >> $temp 
echo "SELECT ShortName FROM Tasks JOIN TaskDefinitions ON Tasks.DefinitionId = TaskDefinitions.Id" | pwnctl query | jq .ShortName -r | sort | uniq -c | sort >> $temp
echo >> $temp 
pwnctl summary >> $temp 
echo >> $temp 
job-queue.sh --status -q $PWNCTL_INSTALL_PATH/queue/ >> $temp

cat $temp | notify -provider discord -id status -bulk

rm $temp