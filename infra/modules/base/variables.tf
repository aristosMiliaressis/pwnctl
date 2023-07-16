variable "profile" {
  description = "The AWS profile to use."
  default = "default"
}

variable "stack_name" {
  description = "Name of the stack."
  type        = string
  default     = "pwnctl"
}

resource "random_id" "nonce" {
  byte_length = 8
}
