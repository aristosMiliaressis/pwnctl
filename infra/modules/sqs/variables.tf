variable "sqs_visibility_timeout" {
  description = "SQS Visibility Timeout"
  type        = number

  default     = 1200
}

variable "profile" {
  type        = string
}

variable "nonce" {
  type        = string
}
