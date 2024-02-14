variable "profile" {
  description = "The local AWS configuration profile to use."
  type        = string

  default     = "default"
}

variable "private_key_location" {
  description = "Location of the private key"
  type        = string

  default     = "pwnctl_jumpbox.pem"
}