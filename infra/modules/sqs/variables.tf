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

variable "shortlived_visibility_timeout" {
  description = "SQS ShortLived queue visibility timeout"
  type        = number
}

variable "longlived_visibility_timeout" {
  description = "SQS LongLived queue visibility timeout"
  type        = number
}

variable "output_visibility_timeout" {
  description = "SQS output queue visibility timeout"
  type        = number
}