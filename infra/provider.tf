
/* provider "digitalocean" {
  token = var.do_token
} */

provider "aws" {
  profile = var.profile
  region = var.region

  default_tags {
    tags = {
      Name     = "pwnctl_${var.pwnctl_id}"
      Stack    = var.stack_name
    }
  }
}