#!/bin/bash

temp=`mktemp`

echo 'SELECT "ShortName" FROM "Tasks" JOIN "TaskDefinitions" ON "Tasks"."DefinitionId" = "TaskDefinitions"."Id"' \
    | pwnctl query \
    | jq .ShortName -r 2>/dev/null \
    | sort \
    | uniq -c \
    | sort -n >> $temp

echo >> $temp 
pwnctl summary >> $temp 
echo >> $temp 

cat $temp | notify -provider discord -id status -bulk

rm $temp