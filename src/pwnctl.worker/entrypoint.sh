#!/bin/bash

# downloading executable in entrypoint to ensure we alway get the latest cli
curl https://raw.githubusercontent.com/aristosMiliaressis/pwntainer/master/src/pwnctl.cli/install.sh | bash
curl -o /usr/local/bin/job-queue.sh https://raw.githubusercontent.com/aristosMiliaressis/job-queue.sh/master/job-queue.sh

# volume mapped entrypoint_hook.sh for injecting resources and 
# running commands at startup without needing to rebuild the image
if test -f "$PWNCTL_INSTALL_PATH/entrypoint_hook.sh"; 
then
    bash "$PWNCTL_INSTALL_PATH/entrypoint_hook.sh"
fi

if ! test -f "/opt/wordlists/dns/resolvers_top25.txt"; 
then
    echo 'get-valid-resolvers.sh' | job-queue.sh -w 1 -q "$PWNCTL_INSTALL_PATH/queue"
fi

while true; do sleep 10000; done