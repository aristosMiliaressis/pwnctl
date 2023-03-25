
/* provider "digitalocean" {
  token = var.do_token
} */

provider "aws" {
  profile = var.profile
  region = var.region

  default_tags {
    tags = {
      Name     = "pwnctl_${random_id.id.hex}"
      Stack    = var.stack_name
    }
  }
}