
variable "profile" {
  description = "The local AWS configuration profile to use."
  type        = string

  default = "default"
}

variable "access_timeout_minutes" {
  description = "The access token expiration in minutes."
  type        = number

  default = 120
}

variable "refresh_timeout_hours" {
  description = "The refresh token expiration in hours."
  type        = number

  default = 720
}

variable "ecs_cluster" {
  default = {
    name = "pwnctl_cluster"
  }
}

variable "ecs_service" {
  default = {
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