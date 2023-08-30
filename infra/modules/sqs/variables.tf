variable "profile" {
  description = "The local AWS configuration profile to use."
  type        = string
}

variable "env" {
  description = "Environment identifier"
  type        = string
}

variable "message_retention_seconds" {
  description = "Message retention in seconds."
  type        = number
}

variable "sqs_visibility_timeout" {
  description = "SQS Visibility Timeout"
  type        = number

  default     = 1200
}