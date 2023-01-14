#!/bin/bash

temp=`mktemp`

echo 'SELECT "ShortName" FROM "TaskEntries" JOIN "TaskDefinitions" ON "TaskEntries"."DefinitionId" = "TaskDefinitions"."Id"' \
    | pwnctl query \
    | jq '.[].ShortName' -r 2>/dev/null \
    | sort \
    | uniq -c \
    | sort -n >> $temp

echo >> $temp 
pwnctl summary >> $temp 
echo >> $temp 

cat $temp | /root/go/bin/notify -provider discord -id status -bulk

rm $temp