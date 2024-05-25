locals  {
  access_timeout_minutes = 120
  refresh_timeout_hours = 720
}

data "aws_vpc" "this" {
  id = var.vpc_id
}

data "archive_file" "this" {
  type        = "zip"
  source_dir  = "../src/pwnctl.api/bin/Release/net6.0/"
  output_path = "../src/pwnctl.api/bin/lambda.zip"
}

resource "aws_lambda_function" "this" {
  filename         = "../src/pwnctl.api/bin/lambda.zip"
  function_name    = "pwnctl_api"
  role             = aws_iam_role.lambda.arn
  handler          = "pwnctl.api"
  source_code_hash = data.archive_file.this.output_base64sha256
  runtime          = "dotnet6"
  timeout          = 300
  memory_size      = 2048

  vpc_config {
    subnet_ids         = [var.public_subnet_a, var.public_subnet_b]
    security_group_ids = [aws_security_group.allow_https_from_internet.id]
  }

  file_system_config {
    arn              = aws_efs_access_point.this.arn
    local_mount_path = local.efs_mount_point
  }

  environment {
    variables = {
      PWNCTL_LongLivedTaskQueue__Name  = module.sqs.longlived_tasks_queue.name,
      PWNCTL_ShortLivedTaskQueue__Name = module.sqs.shortlived_tasks_queue.name,
      PWNCTL_Logging__MinLevel         = "Debug"
      PWNCTL_Api__AccessTimeoutMinutes = tostring(local.access_timeout_minutes)
      PWNCTL_Api__RefreshTimeoutHours  = tostring(local.refresh_timeout_hours)
      PWNCTL_Db__Name                  = var.db_name
      PWNCTL_Db__Username              = var.db_user
      PWNCTL_Db__Host                  = var.db_host
      PWNCTL_FS_MOUNT_POINT            = local.efs_mount_point
    }
  }
  
  depends_on = [
    aws_efs_mount_target.this,
    aws_security_group.allow_https_from_internet,
    aws_iam_role.lambda
  ]
  
  tags = {
    Name = "pwnctl_lambda"
  }
}

resource "aws_security_group" "allow_https_from_internet" {
  name        = "allow_https_from_internet"
  description = "Allow HTTPS inbound traffic from anywhere"
  vpc_id      = var.vpc_id

  ingress {
    description      = "Allow HTTPS inbound traffic from anywhere"
    from_port        = 443
    to_port          = 443
    protocol         = "tcp"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }

  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}

resource "aws_lambda_function_url" "this" {
  function_name      = aws_lambda_function.this.function_name
  authorization_type = "NONE"
}

resource "aws_ssm_parameter" "api_url" {
  name  = "/pwnctl/Api/BaseUrl"
  type  = "String"
  value = aws_lambda_function_url.this.function_url
}

data "aws_network_interface" "a" {
  filter {
    name = "subnet-id"

    values = [var.public_subnet_a]
  }

  filter {
    name = "group-id"

    values = [aws_security_group.allow_https_from_internet.id]
  }

  filter {
    name = "interface-type"

    values = ["lambda"]
  }

  depends_on = [aws_lambda_function.this]
}

data "aws_network_interface" "b" {
  filter {
    name = "subnet-id"

    values = [var.public_subnet_b]
  }

  filter {
    name = "group-id"

    values = [aws_security_group.allow_https_from_internet.id]
  }

  filter {
    name = "interface-type"

    values = ["lambda"]
  }

  depends_on = [aws_lambda_function.this]
}

resource "aws_eip" "a" {
  domain            = "vpc"
  network_interface = data.aws_network_interface.a.id
}

resource "aws_eip" "b" {
  domain            = "vpc"
  network_interface = data.aws_network_interface.b.id
}

output "api_url" {
  value = aws_lambda_function_url.this.function_url
}
