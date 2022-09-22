#!/bin/bash

# downloading executable in entrypoint to ensure we alway get the latest cli
curl https://raw.githubusercontent.com/aristosMiliaressis/pwntainer/master/src/pwnctl.cli/install.sh | bash

# volume mapped entrypoint_hook.sh for injecting resources and 
# running commands at startup without needing to rebuild the image
if test -f "$PWNCTL_INSTALL_PATH/entrypoint_hook.sh"; 
then
    bash "$PWNCTL_INSTALL_PATH/entrypoint_hook.sh"
fi

while true; do sleep 10000; done