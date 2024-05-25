
# EventBridge Role
data "aws_iam_policy_document" "event_publisher" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["events.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "event_publisher" {
  name               = "event_publisher"
  assume_role_policy = data.aws_iam_policy_document.event_publisher.json
}

data "aws_iam_policy_document" "ecs_events_run_task_with_any_role" {
  statement {
    effect    = "Allow"
    actions   = ["iam:PassRole"]
    resources = ["*"]
  }

  statement {
    effect    = "Allow"
    actions   = ["ecs:RunTask"]
    resources = [replace(aws_ecs_task_definition.proc.arn, "/:\\d+$/", ":*")]
  }
}

resource "aws_iam_role_policy" "ecs_events_run_task_with_any_role" {
  name   = "ecs_events_run_task_with_any_role"
  role   = aws_iam_role.event_publisher.id
  policy = data.aws_iam_policy_document.ecs_events_run_task_with_any_role.json
}

# Lambda Execution role
data "aws_iam_policy" "eventbridge_scheduler" {
  name = "eventbridge_scheduler"
}

data "aws_iam_policy" "sqs_readwrite" {
  name = "sqs-readwrite"
}

resource "aws_iam_role_policy_attachment" "grant_lambda_eventbridge_scheduler_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.eventbridge_scheduler.arn
}

data "aws_iam_policy" "lambda_basic_execution" {
  name = "AWSLambdaBasicExecutionRole"
}

data "aws_iam_policy" "lambda_vpc_access" {
  name = "AWSLambdaVPCAccessExecutionRole"
}

resource "aws_iam_role" "lambda" {
  name = "pwnctl_lambda_service_role"

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
    Name = "pwnctl_lambda_role"
  }
}

data "aws_iam_policy_document" "cloud_watch_logs_access" {
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

resource "aws_iam_policy" "cloud_watch_logs_access" {
  name        = "cloud_watch_logs_access"
  path        = "/"
  description = "IAM policy for logging from a lambda"
  policy      = data.aws_iam_policy_document.cloud_watch_logs_access.json
}

resource "aws_iam_role_policy_attachment" "grant_lambda_basic_execution" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.lambda_basic_execution.arn
}

resource "aws_iam_role_policy_attachment" "grant_lambda_vpc_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.lambda_vpc_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_lambda_efs_client_full_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.efs_client_full_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_lambda_ssm_readonly_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.ssm_readonly_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_lambda_sm_readwrite_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.sm_readwrite_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_lambda_sqs_readwrite_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = data.aws_iam_policy.sqs_readwrite.arn
}

resource "aws_iam_role_policy_attachment" "grant_lambda_cloud_watch_logs_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = aws_iam_policy.cloud_watch_logs_access.arn
}

##

data "aws_iam_policy" "efs_client_full_access" {
  name = "AmazonElasticFileSystemClientFullAccess"
}

data "aws_iam_policy" "ssm_readonly_access" {
  name = "AmazonSSMReadOnlyAccess"
}

data "aws_iam_policy" "sm_readwrite_access" {
  name = "SecretsManagerReadWrite"
}

data "aws_iam_policy" "ecs_task_execution" {
  name = "AmazonECSTaskExecutionRolePolicy"
}

data "aws_iam_policy" "ec2_container_registry_readonly" {
  name = "AmazonEC2ContainerRegistryReadOnly"
}

data "aws_iam_policy" "cloud_watch_logs_full_access" {
  name = "CloudWatchLogsFullAccess"
}

data "aws_iam_policy" "rds_full_access" {
  name = "AmazonRDSFullAccess"
}