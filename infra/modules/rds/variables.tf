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