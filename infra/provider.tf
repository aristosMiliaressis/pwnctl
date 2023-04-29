
provider "aws" {
  profile = var.profile
  region = var.region

  default_tags {
    tags = {
      Name     = "pwnctl_${random_id.id.hex}"
    }
  }
}