
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

resource "aws_ecs_task_definition" "this" {
  family = "service"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 3072
  execution_role_arn = aws_iam_role.ecs.arn
  task_role_arn = aws_iam_role.ecs.arn
  container_definitions = templatefile("task-definitions/pwnctl.tftpl", {
    efs_mount_point = var.efs_mount_point
    efs_name = "pwnctl-fs",
    db_name = var.rds_postgres_databasename,
    db_username = var.rds_postgres_username,
    db_password = random_password.db.result
    db_endpoint = aws_db_instance.this.endpoint,
    sqs_visibility_timeout = var.sqs_visibility_timeout,
    sqs_queue_name = aws_sqs_queue.main.name
  })

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

# Queue.GrantConsumeMessages
# Queue.GrantSendMessages
# DatabaseSecret.GrantRead
#FileSystem.Connections.AllowDefaultPortFrom(FargateService);
#Database.Connections.AllowDefaultPortFrom(FargateService);

# READ: https://www.terraform.io/language/functions/templatefile
# Container LogGroup

resource "aws_ecs_cluster" "this" {
  name = "pwnctl-cluster"
}

resource "aws_ecs_service" "this" {
  name            = "pwnctl-svc"
  launch_type     = "FARGATE"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.this.arn
  desired_count   = 0
  iam_role        = aws_iam_role.ecs.arn
  depends_on      = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.this, 
    aws_iam_role.ecs
  ]

  network_configuration {
    subnets = [for k, v in aws_subnet.private : aws_subnet.private[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    weight = 1
  }

  /*ordered_placement_strategy {
    type  = "binpack"
    field = "cpu"
  }

  placement_constraints {
    type       = "memberOf"
    expression = "attribute:ecs.availability-zone in [us-west-2a, us-west-2b]"
  } */
}