data "archive_file" "this" {
  type = "zip"
  source_dir = "../src/pwnctl.api/bin/Release/net6.0/"
  output_path = "../src/pwnctl.api/bin/Release/net6.0/lambda.zip"
}

resource "aws_iam_role" "this" {
  name = "pwnctl_lambda_role_${var.pwnctl_id}_service_role"
  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
  tags = {
    Name = "pwnctl_lambda_role_${var.pwnctl_id}"
    Stack = var.stack_name
  }
}

resource "aws_cloudwatch_log_group" "this" {
  name              = "/aws/lambda/${var.stack_name}"
  retention_in_days = 7
}

resource "aws_lambda_function" "this" {
  tags = {
    Name = "pwnctl_lambda_${var.pwnctl_id}"
    Stack = var.stack_name
  }

  depends_on = [
    aws_efs_mount_target.this,
    aws_cloudwatch_log_group.this,
    aws_iam_role.this
  ]

  filename = "../src/pwnctl.api/bin/Release/net6.0/lambda.zip"
  function_name = "pwnctl_api_${var.pwnctl_id}"
  role = aws_iam_role.this.arn
  handler = "pwnctl.api"
  source_code_hash = data.archive_file.this.output_base64sha256
  runtime = "dotnet6"
  timeout = 120
  memory_size = 3072
  
  vpc_config {
    subnet_ids         = [for k, v in aws_subnet.private : aws_subnet.private[k].id]
    security_group_ids = [aws_security_group.allow_https_from_internet.id]
  }

  file_system_config {
    arn = aws_efs_access_point.this.arn
    local_mount_path = "/mnt/efs"
  }

  environment {
      variables = {
          PWNCTL_Aws__InVpc = "true"
          PWNCTL_TaskQueue__QueueName = ""
          PWNCTL_TaskQueue__DLQName = ""
          PWNCTL_TaskQueue__VisibilityTimeout = ""
          PWNCTL_Logging__MinLevel = "Debug"
          PWNCTL_Logging__FilePath = "/mnt/efs/"
          PWNCTL_Logging__LogGroup = ""
          PWNCTL_INSTALL_PATH = "/mnt/efs/"
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

  tags = {
    Name = "allow_https_from_internet"
  }
}