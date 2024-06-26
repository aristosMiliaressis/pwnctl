locals {
  efs_mount_point = "/mnt/efs"
}

resource "aws_efs_file_system" "this" {
  tags = {
    Description = "PwnCtl Elastic File System."
  }
}

resource "aws_efs_mount_target" "this" {
  for_each = {
    a = var.public_subnet_a
    b = var.public_subnet_b
  }

  file_system_id  = aws_efs_file_system.this.id
  subnet_id       = each.value
  security_groups = [aws_security_group.allow_nfs.id]
}

resource "aws_efs_access_point" "this" {
  file_system_id = aws_efs_file_system.this.id

  root_directory {
    path = "/"
    creation_info {
      owner_gid   = 1001
      owner_uid   = 1001
      permissions = "777"
    }
  }

  posix_user {
    gid = 0
    uid = 0
  }
}

resource "aws_security_group" "allow_nfs" {
  name        = "allow-nfs"
  description = "Allow ingress NFS traffic from VPC"
  vpc_id      = var.vpc_id

  ingress {
    description = "Allow ingress NFS traffic from VPC"
    from_port   = 2049
    to_port     = 2049
    protocol    = "tcp"
    cidr_blocks = [data.aws_vpc.this.cidr_block]
  }

  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}