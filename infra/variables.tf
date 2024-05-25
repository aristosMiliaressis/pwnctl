
variable "profile" {
  description = "The local AWS configuration profile to use."
  type        = string

  default = "default"
}

variable "vpc_id" {
  description = "The VPC id."
  type        = string
}

variable "public_subnet_a" {
  description = "The public subnet A id."
  type        = string
}

variable "public_subnet_b" {
  description = "The public subnet B id."
  type        = string
}

variable "private_subnet_a" {
  description = "The private subnet A id."
  type        = string
}

variable "private_subnet_b" {
  description = "The private subnet B id."
  type        = string
}

variable "db_host" {
  description = "Database host."
  type        = string
}

variable "db_name" {
  description = "Database name."
  type        = string
}

variable "db_user" {
  description = "Database user."
  type        = string
}

variable "exec_instance_count" {
  description = "The maximum number of concurrent exec instances."

  default = {
    longlived  = 30
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
    longlived  = 3
    shortlived = 2
  }
}

variable "task_timeout" {
  description = "The max amount of seconds a task execution may take before timing out."
  type        = number

  default = 10800 # 3 hours
}

