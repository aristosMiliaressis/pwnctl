terraform {
  required_version = ">= 0.14"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 5.0.0"
    }
    docker = {
      source = "kreuzwerker/docker"
    }
    random = {
      source = "hashicorp/random"
    }
  }
}

data "aws_caller_identity" "current" {}
data "aws_ecr_authorization_token" "token" {}
data "external" "aws_region" {
  program = ["bash", "-c", "aws configure get region | jq --raw-input '. | { region: (.) }'"]
}

provider "aws" {
  profile = var.profile
  region = data.external.aws_region.result.region

  default_tags {
    tags = {
      Name     = "pwnctl_${var.nonce}"
    }
  }
}

provider "docker" {
  registry_auth {
    address  = format("%v.dkr.ecr.%v.amazonaws.com", data.aws_caller_identity.current.account_id, data.external.aws_region.result.region)
    username = data.aws_ecr_authorization_token.token.user_name
    password = data.aws_ecr_authorization_token.token.password
  }
}
