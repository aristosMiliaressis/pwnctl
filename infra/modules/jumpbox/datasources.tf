
data "aws_vpc" "main" {
  tags = {
    Name = "PwnCtl VPC"
  }
}

data "aws_efs_file_system" "main" {
  tags = {
    Description = "PwnCtl Elastic File System."
  }
}

data "aws_subnet" "primary" {
  tags = {
    Name = "PwnCtl Public Subnet a"
  }
}

data "aws_ami" "amazon_linux_ami" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name = "name"
    values = [
      "amzn2-ami-kernel-*-hvm-*-x86_64-gp2"
    ]
  }
}