
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

variable "pwnctl_id" {
  description = "variable for unique naming."
}

variable "stack_name" {
  description = "Name of the stack."
  default = "PwnCtl"
}
