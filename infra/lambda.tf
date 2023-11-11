data "archive_file" "this" {
  type = "zip"
  source_dir = "../src/pwnctl.api/bin/Release/net6.0/"
  output_path = "../src/pwnctl.api/bin/lambda.zip"
}

resource "aws_lambda_function" "this" {
  tags = {
    Name = "pwnctl_lambda"
  }

  depends_on = [
    aws_efs_mount_target.this,
    aws_security_group.allow_https_from_internet,
    aws_security_group.allow_postgres,
    aws_iam_role.lambda
  ]

  filename = "../src/pwnctl.api/bin/lambda.zip"
  function_name = "pwnctl_api"
  role = aws_iam_role.lambda.arn
  handler = "pwnctl.api"
  source_code_hash = data.archive_file.this.output_base64sha256
  runtime = "dotnet6"
  timeout = 300
  memory_size = 3008

  vpc_config {
    subnet_ids         = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    security_group_ids = [aws_security_group.allow_https_from_internet.id]
  }

  file_system_config {
    arn = aws_efs_access_point.this.arn
    local_mount_path = var.efs_mount_point
  }

  environment {
      variables = {
          PWNCTL_TaskQueue__Name = module.sqs.main_queue.name,
          PWNCTL_TaskQueue__VisibilityTimeout = tostring(module.sqs.sqs_visibility_timeout),
          PWNCTL_Logging__MinLevel = "Debug"
          PWNCTL_Api__AccessTimeoutMinutes = tostring(var.access_timeout_minutes)
          PWNCTL_Api__RefreshTimeoutHours = tostring(var.refresh_timeout_hours)
          PWNCTL_Db__Name = var.rds_postgres_databasename
          PWNCTL_Db__Username = var.rds_postgres_username
          PWNCTL_Db__Host = aws_db_instance.this.endpoint
          PWNCTL_INSTALL_PATH = var.efs_mount_point
      }
  }
}

resource "aws_security_group" "allow_https_from_internet" {
  name        = "allow_https_from_internet"
  description = "Allow HTTPS inbound traffic from anywhere"
  vpc_id      = aws_vpc.main.id

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

  depends_on = [
    aws_vpc.main,
    aws_subnet.public
  ]
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

output "api_url" {
  value = aws_lambda_function_url.this.function_url
}
