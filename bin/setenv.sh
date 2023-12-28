#!/bin/bash

export AWS_REGION=$(aws configure get region)
export AWS_ACCOUNT_ID=$(aws sts get-caller-identity --query "Account" --output text)
export PWNCTL_PROC_TAG=$(docker images | grep pwnctl-proc | head -n 1 | awk '{print $2}')
export PWNCTL_EXEC_SHORT_TAG=$(docker images | grep pwnctl-exec-short | head -n 1 | awk '{print $2}')
export PWNCTL_EXEC_LONG_TAG=$(docker images | grep pwnctl-exec-long | head -n 1 | awk '{print $2}')
