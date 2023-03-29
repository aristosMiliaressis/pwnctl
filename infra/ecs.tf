# READ: https://github.com/hashicorp/terraform-provider-aws/issues/25794

resource "aws_iam_role" "ecs" {
  name = "pwnctl_${random_id.id.hex}_ecs_service_role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      },
    ]
  })

  tags = {
    Name = "pwnctl_ecs_role_${random_id.id.hex}"
  }
}

data "aws_iam_policy" "ecs_task_execution_role_policy" {
  name = "AmazonECSTaskExecutionRolePolicy"
}

data "aws_iam_policy" "efs_client_full_access_to_ecs" {
  name = "AmazonElasticFileSystemClientFullAccess"
}

data "aws_iam_policy" "ec2_container_registry_read_only" {
  name = "AmazonEC2ContainerRegistryReadOnly"
}

data "aws_iam_policy" "cloud_watch_logs_full_access" {
  name = "CloudWatchLogsFullAccess"
}

data "aws_iam_policy" "rds_full_access" {
  name = "AmazonRDSFullAccess"
}

resource "aws_iam_role_policy_attachment" "attach_ecs_task_execution_role_policy" {
  role       = aws_iam_role.ecs.name
  policy_arn = data.aws_iam_policy.ecs_task_execution_role_policy.arn
}
resource "aws_iam_role_policy_attachment" "attach_efs_client_full_access_to_ecs" {
  role       = aws_iam_role.ecs.name
  policy_arn = data.aws_iam_policy.efs_client_full_access_to_ecs.arn
}
resource "aws_iam_role_policy_attachment" "attach_ec2_container_registry_read_only" {
  role       = aws_iam_role.ecs.name
  policy_arn = data.aws_iam_policy.ec2_container_registry_read_only.arn
}
resource "aws_iam_role_policy_attachment" "attach_cloud_watch_logs_full_access" {
  role       = aws_iam_role.ecs.name
  policy_arn = data.aws_iam_policy.cloud_watch_logs_full_access.arn
}
resource "aws_iam_role_policy_attachment" "attach_rds_full_access" {
  role       = aws_iam_role.ecs.name
  policy_arn = data.aws_iam_policy.rds_full_access.arn
}

# Queue.GrantConsumeMessages
# Queue.GrantSendMessages
# DatabaseSecret.GrantRead

resource "aws_ecs_task_definition" "this" {
  family = "service"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 3072
  execution_role_arn = aws_iam_role.ecs.arn
  task_role_arn = aws_iam_role.ecs.arn
  container_definitions = file("task-definitions/pwnctl.json")

  volume {
    name      = "pwnctl-fs"
    efs_volume_configuration {
        file_system_id = aws_efs_file_system.this.id
        root_directory = "/"
        transit_encryption = "ENABLED"
        authorization_config {
            iam = "ENABLED"
            access_point_id = aws_efs_access_point.this.id
        }
    }
  }
}

/* data "aws_ecr_image" "this" {
  repository_name = "i0m2p7r6/pwnctl"
  image_tag       = "latest"
} */

# Container LogGroup
# Container MountPoint
# StopTimeout

/* data "aws_ecs_container_definition" "pwnctl" {
  task_definition = aws_ecs_task_definition.this.id
  container_name  = "pwnctl"
  image = aws_ecr_image.this

  environment {
      variables = {
          PWNCTL_Aws__InVpc = "true"
          PWNCTL_TaskQueue__QueueName = "pwnctl_${random_id.id.hex}.fifo"
          PWNCTL_TaskQueue__DLQName = "pwnctl_${random_id.id.hex}_dlq.fifo"
          PWNCTL_TaskQueue__VisibilityTimeout = var.sqs_visibility_timeout
          PWNCTL_Logging__MinLevel = "Debug"
          PWNCTL_Logging__FilePath = var.efs_mount_point
          #PWNCTL_Logging__LogGroup = "/aws/ecs/${var.stack_name}"
          PWNCTL_Db__Name = var.rds_postgres_databasename
          PWNCTL_Db__Username = var.rds_postgres_username
          PWNCTL_Db__Password = random_password.db.result
          PWNCTL_Db__Host = aws_db_instance.this.endpoint
          PWNCTL_INSTALL_PATH = var.efs_mount_point
      }
  }
} */

resource "aws_ecs_cluster" "this" {
  name = "pwnctl-cluster"
}

/* resource "aws_ecs_service" "this" {
  name            = "mongodb"
  launch_type     = "FARGATE"
  cluster         = aws_ecs_cluster.foo.id
  task_definition = aws_ecs_task_definition.mongo.arn
  desired_count   = 3
  iam_role        = aws_iam_role.foo.arn
  depends_on      = [aws_iam_role_policy.foo]

  ordered_placement_strategy {
    type  = "binpack"
    field = "cpu"
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.foo.arn
    container_name   = "mongo"
    container_port   = 8080
  }

  placement_constraints {
    type       = "memberOf"
    expression = "attribute:ecs.availability-zone in [us-west-2a, us-west-2b]"
  }
} */