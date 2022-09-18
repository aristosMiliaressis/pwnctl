#!/bin/bash

pwnctl summary && echo \
    && job-queue.sh --status -q $PWNCTL_INSTALL_PATH/jobs/ -w $PWNCTL_BASH_WORKERS \
    | notify -provider discord -id status -bulk