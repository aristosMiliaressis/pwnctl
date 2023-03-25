
variable "do_token" {
  description = "The DigitalOcean token."
  default = ""
}

variable "profile" {
  description = "The AWS profile to use."
  default = "r00t"
}

variable "region" {
  description = "The AWS region to deploy resources to."
  default = "us-east-1"
}

resource "random_id" "id" {
  byte_length = 8
}

variable "stack_name" {
  description = "Name of the stack."
  default = "PwnCtl"
}

variable "sqs_visibility_timeout" {
  description = "SQS Visibility Timeout"
  default = 600
}
