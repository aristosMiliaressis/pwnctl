locals {
  mount_point = "/mnt/efs"
}

resource "tls_private_key" "ssh" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "local_file" "private_key" {
  content         = tls_private_key.ssh.private_key_pem
  filename        = abspath(var.private_key_location)
  file_permission = "0400"
}

resource "aws_key_pair" "this" {
  key_name   = "pwnctl-jumpbox-key"
  public_key = tls_private_key.ssh.public_key_openssh
}

resource "aws_instance" "this" {
  ami                         = data.aws_ami.amazon_linux_ami.id
  instance_type               = "t2.micro"
  associate_public_ip_address = true
  vpc_security_group_ids      = [aws_security_group.this.id]
  key_name                    = aws_key_pair.this.key_name
  subnet_id                   = data.aws_subnet.primary.id

  provisioner "file" {
    source      = "efs_mount.sh"
    destination = "efs_mount.sh"
  }

  connection {
    type        = "ssh"
    host        = self.public_ip
    user        = "ec2-user"
    private_key = tls_private_key.ssh.private_key_pem
    timeout     = "4m"
  }

  provisioner "remote-exec" {
    inline = [
      "bash efs_mount.sh",
    ]
  }

  depends_on = [
    null_resource.generate_efs_mount_script
  ]
}

resource "aws_security_group" "this" {
  name = "pwnctl-jumpbox"
  vpc_id      = data.aws_vpc.main.id
  
  ingress = [
    {
      from_port        = 22
      to_port          = 22
      protocol         = "tcp"
      cidr_blocks      = ["0.0.0.0/0"]
      ipv6_cidr_blocks = ["::/0"]
      description      = "allow-ssh"
      prefix_list_ids  = []
      self             = false
      security_groups  = []
    }
  ]

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "null_resource" "generate_efs_mount_script" {

  provisioner "local-exec" {
    command = templatefile("generate_fs_mount_script.sh", {
      efs_mount_point = local.mount_point
      file_system_id  = data.aws_efs_file_system.main.id
    })
    interpreter = [
      "bash",
      "-c"
    ]
  }
}

resource "null_resource" "clean_up" {

  provisioner "local-exec" {
    when    = destroy
    command = "rm -rf efs_mount.sh"
  }
}