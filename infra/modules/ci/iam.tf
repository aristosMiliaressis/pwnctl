## Data Sources

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

# ECS Execution role
resource "aws_iam_role" "ecs_service" {
  name = "pwnctl-ecs-service"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
          AWS = [
            data.external.caller_identity.result.Arn,
          ]
        }
      }
    ]
  })
}

data "aws_iam_policy_document" "scalein_protection" {
  statement {
    effect = "Allow"

    actions = [
      "ecs:UpdateTaskProtection"
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "scalein_protection" {
  name        = "scalein-protection"
  path        = "/"
  description = "IAM policy for ECS Scale in Protection"
  policy      = data.aws_iam_policy_document.scalein_protection.json
}

resource "aws_iam_role_policy_attachment" "grant_ecs_scalein_protection_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = aws_iam_policy.scalein_protection.arn
}

data "aws_iam_policy_document" "sqs_readwrite" {
  statement {
    effect = "Allow"

    actions = [
      "sqs:ChangeMessageVisibility",
      "sqs:DeleteMessage",
      "sqs:GetQueueAttributes",
      "sqs:GetQueueUrl",
      "sqs:ReceiveMessage",
      "sqs:SendMessage"
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "sqs_readwrite" {
  name        = "sqs-readwrite"
  path        = "/"
  description = "IAM policy for sqs Read/Write access"
  policy      = data.aws_iam_policy_document.sqs_readwrite.json
}

resource "aws_iam_role_policy_attachment" "grant_ecs_sqs_readwrite_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = aws_iam_policy.sqs_readwrite.arn
}

resource "aws_iam_role_policy_attachment" "grant_ecs_task_execution" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.ecs_task_execution.arn
}
resource "aws_iam_role_policy_attachment" "grant_ecs_efs_client_full_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.efs_client_full_access.arn
}
resource "aws_iam_role_policy_attachment" "grant_ecs_ec2_container_registry_readonly_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.ec2_container_registry_readonly.arn
}
resource "aws_iam_role_policy_attachment" "grant_ecs_cloud_watch_logs_full_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.cloud_watch_logs_full_access.arn
}
resource "aws_iam_role_policy_attachment" "grant_ecs_rds_full_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.rds_full_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_ecs_ssm_readonly_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.ssm_readonly_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_ecs_sm_readwrite_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = data.aws_iam_policy.sm_readwrite_access.arn
}

resource "aws_iam_role_policy_attachment" "grant_eventbridge_scheduler_access" {
  role       = aws_iam_role.ecs_service.name
  policy_arn = aws_iam_policy.eventbridge_scheduler.arn
}

data "aws_iam_policy_document" "eventbridge_scheduler" {
  statement {
    effect = "Allow"

    actions = [
      "ecs:ListClusters",
      "ecs:ListTaskDefinitions",
      "iam:ListRoles",
      "iam:PassRole",
      "events:PutRule",
      "events:PutTargets",
      "events:DeleteRule",
      "events:DeleteTargets",
      "events:RemoveTargets",
      "events:ListTargetsByRule"
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "eventbridge_scheduler" {
  name        = "eventbridge_scheduler"
  path        = "/"
  description = "IAM policy to create & delete event bridge schedules"
  policy      = data.aws_iam_policy_document.eventbridge_scheduler.json
}