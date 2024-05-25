locals {
  db_name = "pwnctl"
  db_user = "pwnadmin"
}

data "aws_vpc" "this" {
  id = var.vpc_id
}

resource "aws_db_subnet_group" "this" {
  name       = "main"
  subnet_ids = [var.private_subnet_a, var.private_subnet_b]

  tags = {
    Name = "PwnCtl db subnet group"
  }
}

resource "aws_security_group" "allow_postgres" {
  name        = "allow_postgres"
  description = "Allow ingress Postgres traffic from VPC"
  vpc_id      = var.vpc_id

  ingress {
    description = "Allow ingress Postgres traffic from VPC"
    from_port   = 5432
    to_port     = 5432
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

  tags = {
    Name = "allow_postgres"
  }
}

resource "aws_db_parameter_group" "this" {
  name   = "pg-params"
  family = "postgres15"

  parameter {
    name  = "log_connections"
    value = "1"
  }
}

resource "aws_db_instance" "this" {
  allocated_storage      = 10
  engine                 = "postgres"
  engine_version         = "15"
  instance_class         = "db.t3.micro"
  db_name                = local.db_name
  username               = local.db_user
  password               = aws_secretsmanager_secret_version.db_password.secret_string
  parameter_group_name   = aws_db_parameter_group.this.name
  skip_final_snapshot    = true
  vpc_security_group_ids = [aws_security_group.allow_postgres.id]
  db_subnet_group_name   = aws_db_subnet_group.this.id

  depends_on = [
    aws_db_parameter_group.this,
    aws_db_subnet_group.this,
    aws_security_group.allow_postgres
  ]
}
