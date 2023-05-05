
variable "profile" {
  description = "The AWS profile to use."
  default = "default"
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

variable "efs_mount_point" {
  description = "EFS Mount Point."
  default = "/mnt/efs"
}

variable "ecs_cluster" {
  default = {
    name = "pwnctl_cluster"
  }
}

variable "ecs_service" {
  default = {
    name = "pwnctl_svc"
    min_capacity = 0
    max_capacity = 30
  }
}

variable "task_timeout" {
  description = "The max amount of seconds a task execution may take before timing out."
  default = 7200
}

variable "access_timeout_minutes" {
  description = "The access token expiration in minutes."
  default = 120
}

variable "refresh_timeout_hours" {
  description = "The refresh token expiration in hours."
  default = 720
}

variable "admin_password" {
  description = "A password used to seed the default admin user."
}
