variable "profile" {
  description = "The local AWS configuration profile to use."
  type        = string
}

variable "env" {
  description = "Environment identifier"
  type        = string
}

variable "sqs_visibility_timeout" {
  description = "SQS Visibility Timeout"
  type        = number

  default     = 1200
}