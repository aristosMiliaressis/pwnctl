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

resource "aws_cloudwatch_log_group" "this" {
  name              = "/aws/lambda/pwnctl"
  retention_in_days = 7
  lifecycle {
    prevent_destroy = false
  }
}

data "aws_iam_policy_document" "api_logging" {
  statement {
    effect = "Allow"

    actions = [
      "logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
    ]

    resources = ["arn:aws:logs:*:*:*"]
  }
}

resource "aws_iam_policy" "api_logging" {
  name        = "api_logging"
  path        = "/"
  description = "IAM policy for logging from a lambda"
  policy      = data.aws_iam_policy_document.api_logging.json
}

resource "aws_iam_role_policy_attachment" "api_logging" {
  role       = aws_iam_role.lambda.name
  policy_arn = aws_iam_policy.api_logging.arn
}

data "archive_file" "this" {
  type = "zip"
  source_dir = "../src/pwnctl.api/bin/Release/net6.0/"
  output_path = "../src/pwnctl.api/bin/Release/net6.0/lambda.zip"
}

resource "aws_lambda_function" "this" {
  tags = {
    Name = "pwnctl_lambda_${random_id.id.hex}"
  }

  depends_on = [
    aws_efs_mount_target.this,
    aws_iam_role_policy_attachment.api_logging,
    aws_cloudwatch_log_group.this,
    aws_security_group.allow_https_from_internet,
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
    local_mount_path = var.efs_mount_point
  }

  environment {
      variables = {
          PWNCTL_IN_VPC = "true"
          PWNCTL_Worker__MaxTaskTimeout = tostring(var.ecs_task.max_timeout),
          PWNCTL_TaskQueue__Name = aws_sqs_queue.main.name, 
          PWNCTL_TaskQueue__VisibilityTimeout = tostring(var.sqs_visibility_timeout),
          PWNCTL_OutputQueue__Name = aws_sqs_queue.output.name,
          PWNCTL_OutputQueue__VisibilityTimeout = tostring(var.sqs_visibility_timeout), 
          PWNCTL_Logging__MinLevel = "Debug"
          PWNCTL_Logging__FilePath = var.efs_mount_point
          PWNCTL_Logging__LogGroup = "/aws/lambda/${var.stack_name}"
          PWNCTL_Db__Name = var.rds_postgres_databasename
          PWNCTL_Db__Username = var.rds_postgres_username
          PWNCTL_Db__Password = aws_secretsmanager_secret_version.password.secret_string
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
    aws_subnet.private
  ]

  lifecycle {
    create_before_destroy = true
  }

  tags = {
    Name = "allow_https_from_internet"
  }
}

resource "aws_lambda_function_url" "this" {
  function_name      = aws_lambda_function.this.function_name
  authorization_type = "AWS_IAM"
}

resource "aws_ssm_parameter" "api_url" {
  name  = "/pwnctl/Api/BaseUrl"
  type  = "String"
  value = aws_lambda_function_url.this.function_url
}

output "api_url" {
  value = aws_lambda_function_url.this.function_url
}
