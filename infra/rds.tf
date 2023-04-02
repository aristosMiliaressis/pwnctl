variable "rds_postgres_databasename" {
  default = "pwnctl"
}

variable "rds_postgres_username" {
  default = "pwnadmin"
}

resource "random_password" "db" {
  length           = 16
  special          = true
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

resource "aws_secretsmanager_secret" "password" {
  name = "/aws/secret/pwnctl/Db"

  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "password" {
  secret_id = aws_secretsmanager_secret.password.id
  secret_string = random_password.db.result
}

resource "aws_db_subnet_group" "this" {
  name       = "main"
  subnet_ids  = [for k, v in aws_subnet.private : aws_subnet.private[k].id]

  tags = {
    Name = "PwnCtl db subnet group"
  }
}

resource "aws_security_group" "allow_postgres" {
  name        = "allow_postgres"
  description = "Allow ingress Postgres traffic from VPC"
  vpc_id      = aws_vpc.main.id

  ingress {
    description      = "Allow ingress Postgres traffic from VPC"
    from_port        = 5432
    to_port          = 5432
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
    Name = "allow_postgres"
  }
}

resource "aws_db_parameter_group" "this" {
  name   = "my-pg"
  family = "postgres15"

  parameter {
    name  = "log_connections"
    value = "1"
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_db_instance" "this" {
  allocated_storage    = 10
  engine               = "postgres"
  engine_version       = "15"
  instance_class       = "db.t3.micro"
  db_name                = var.rds_postgres_databasename
  username               = var.rds_postgres_username
  password               = aws_secretsmanager_secret_version.password.secret_string
  parameter_group_name = aws_db_parameter_group.this.name
  skip_final_snapshot  = true
  vpc_security_group_ids = [aws_security_group.allow_postgres.id]
  db_subnet_group_name   = aws_db_subnet_group.this.id
}