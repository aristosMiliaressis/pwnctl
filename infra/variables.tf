
variable "profile" {
  description = "The AWS profile to use."
  default = "default"
}

resource "random_id" "nonce" {
  byte_length = 8
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

variable "efs_mount_point" {
  description = "EFS Mount Point."
  type        = string

  default     = "/mnt/efs"
}

variable "task_timeout" {
  description = "The max amount of seconds a task execution may take before timing out."
  type        = number

  default     = 7200
}