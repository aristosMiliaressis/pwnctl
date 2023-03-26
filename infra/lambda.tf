data "aws_iam_policy" "lambda_basic_execution" {
  name = "AWSLambdaBasicExecutionRole"
}

data "aws_iam_policy" "lambda_vpc_access" {
  name = "AWSLambdaVPCAccessExecutionRole"
}

data "aws_iam_policy" "efs_client_full_access" {
  name = "AmazonElasticFileSystemClientFullAccess"
}

resource "aws_iam_role" "lambda" {
  name = "pwnctl_${random_id.id.hex}_lambda_service_role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      },
    ]
  })

  tags = {
    Name = "pwnctl_lambda_role_${random_id.id.hex}"
    Stack = var.stack_name
  }
}

resource "aws_iam_role_policy_attachment" "attach_lambda_basic_execution" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.lambda_basic_execution.arn
}

resource "aws_iam_role_policy_attachment" "attach_lambda_vpc_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.lambda_vpc_access.arn
}

resource "aws_iam_role_policy_attachment" "attach_efs_client_full_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.efs_client_full_access.arn
}

data "archive_file" "this" {
  type = "zip"
  source_dir = "../src/pwnctl.api/bin/Release/net6.0/"
  output_path = "../src/pwnctl.api/bin/Release/net6.0/lambda.zip"
}

resource "aws_cloudwatch_log_group" "this" {
  name              = "/aws/lambda/${var.stack_name}"
  retention_in_days = 7
}

resource "aws_lambda_function" "this" {
  tags = {
    Name = "pwnctl_lambda_${random_id.id.hex}"
    Stack = var.stack_name
  }

  depends_on = [
    aws_efs_mount_target.this,
    aws_cloudwatch_log_group.this,
    aws_iam_role.lambda
  ]

  filename = "../src/pwnctl.api/bin/Release/net6.0/lambda.zip"
  function_name = "pwnctl_api_${random_id.id.hex}"
  role = aws_iam_role.lambda.arn
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
          PWNCTL_TaskQueue__QueueName = "pwnctl_${random_id.id.hex}.fifo"
          PWNCTL_TaskQueue__DLQName = "pwnctl_${random_id.id.hex}_dlq.fifo"
          PWNCTL_TaskQueue__VisibilityTimeout = var.sqs_visibility_timeout
          PWNCTL_Logging__MinLevel = "Debug"
          PWNCTL_Logging__FilePath = "/mnt/efs/"
          PWNCTL_Logging__LogGroup = "/aws/lambda/${var.stack_name}"
          PWNCTL_Db__Name = var.rds_postgres_databasename
          PWNCTL_Db__Username = var.rds_postgres_username
          PWNCTL_Db__Password = random_password.db.result
          PWNCTL_Db__Host = "${aws_db_instance.this.id}.${aws_db_instance.this.identifier}.${var.region}.rds.amazonaws.com:5432"
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