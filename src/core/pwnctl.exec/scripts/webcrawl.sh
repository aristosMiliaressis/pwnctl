#!/bin/bash

url=$1
katana_tmp=`mktemp`
temp=`mktemp`
trap "rm $katana_tmp $temp" EXIT

timeout -k 150m "katana --no-sandbox -hl -d 10 -silent -nc -or -ob -j -jsl -jc -kf all -fx -u $url -o $katana_tmp" >/dev/null

cat $katana_tmp | jq -r .request.endpoint | sort -u 2>/dev/null

cat $katana_tmp \
    | jq -c 'select( .response.forms != null ) | .response.forms[] as $form | select( $form.parameters != null) | $form.parameters[] | { asset:($form.action+"?"+.), tags:{form:true,enctype:$form.enctype,method:$form.method} }' \
    | jq -c 'if .tags.method != "GET" then .tags.Type="Body" else . end' 2>/dev/null >> $temp

cat $temp | grep "form\":true" | while read url; do param=$(echo $url | jq -r .asset | cut -d '?' -f2); tags=$(echo $url | jq -c .tags); echo $url | jq -r .asset | sed 's/%/\\\\x/g' | xargs -I {} printf "{}\n" | unfurl format "{\"asset\":\"%s://%a%p?$param\",\"tags\":$tags}"; done | sort -u 2>/dev/null

#TODO: xhr
