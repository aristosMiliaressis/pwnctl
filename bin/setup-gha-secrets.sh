#!/bin/bash

gh auth login
gh secret set AWS_REGION --body "$(aws configure get region)"
gh secret set AWS_CALLER_IDENTITY --body "$(aws sts get-caller-identity | jq -r .Account)"
gh secret set AWS_ACCESS_KEY_ID --body ""
gh secret set AWS_SECRET_ACCESS_KEY --body ""