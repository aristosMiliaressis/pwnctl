#!/bin/bash

cd $PWNCTL_INSTALL_PATH

# volume mapped entrypoint_hook.sh for injecting resources and 
# running commands at startup without needing to rebuild the image
if test -f "entrypoint_hook.sh"; 
then
    bash "entrypoint_hook.sh"
fi

while true; do sleep 10000; done