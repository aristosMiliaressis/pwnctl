#!/bin/bash

# downloading executable in entrypoint to ensure we always get the latest
curl https://raw.githubusercontent.com/aristosMiliaressis/pwnctl/master/src/pwnctl.cli/install.sh | bash
curl -o /usr/local/bin/job-queue.sh https://raw.githubusercontent.com/aristosMiliaressis/job-queue.sh/master/job-queue.sh

# volume mapped entrypoint_hook.sh for injecting resources and 
# running commands at startup without needing to rebuild the image
if test -f "$PWNCTL_INSTALL_PATH/entrypoint_hook.sh"; 
then
    bash "$PWNCTL_INSTALL_PATH/entrypoint_hook.sh"
fi

if ! test -f "/opt/wordlists/dns/public_suffix_list.dat"; 
then
    get-psl.sh
fi

if ! test -f "/opt/wordlists/dns/resolvers_top25.txt"; 
then
    get-valid-resolvers.sh
fi

echo "pwnwrk service started" | notify -provider discord -id status

/opt/pwnwrk/pwnwrk

echo "pwnwrk service stoped" | notify -provider discord -id status
