
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

variable "exec_instance_count" {
  description = "The maximum number of concurrent exec instances."

  default = {
    longlived  = 20
    shortlived = 20
  }
}

variable "exec_step_interval" {
  description = "exec service queue depth scaling interval."

  default = {
    longlived  = 8
    shortlived = 20
  }
}

variable "exec_step_size" {
  description = "exec service scaling step size."

  default = {
    longlived  = 2
    shortlived = 2
  }
}

variable "task_timeout" {
  description = "The max amount of seconds a task execution may take before timing out."
  type        = number

  default = 10800 # 3 hours
}

variable "message_retention_seconds" {
  description = "SQS message retention in seconds."
  type        = number

  default = 1209600 # 14 days
}

variable "message_visibility_timeout" {
  description = "SQS message visibility timeout in seconds."

  default = {
    shortlived = 180
    longlived  = 900
    output     = 450
  }
}
