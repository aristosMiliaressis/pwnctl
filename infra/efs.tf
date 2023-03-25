resource "aws_efs_file_system" "this" {
  tags = {
    Name = "pwnctl_fs_${var.pwnctl_id}"
  }
}

resource "aws_efs_mount_target" "this" {
  file_system_id = aws_efs_file_system.this.id
  subnet_id      = aws_subnet.private["a"].id 

  security_groups = [ aws_security_group.allow_nfs.id ]
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
  name        = "allow_nfs"
  description = "Allow NFS inbound traffic"
  vpc_id      = aws_vpc.main.id

  ingress {
    description      = "NFS from VPC"
    from_port        = 2049
    to_port          = 2049
    protocol         = "tcp"
    cidr_blocks      = [aws_vpc.main.cidr_block]
  }

  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }

  tags = {
    Name = "allow_nfs"
  }
}