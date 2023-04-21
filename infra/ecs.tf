#resource "aws_ecr_repository" "main" {
#  name                 = "${var.name}-${var.environment}"
#  image_tag_mutability = "MUTABLE"
#}

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
      }
    ]
  })

  tags = {
    Name = "pwnctl_ecs_role_${random_id.id.hex}"
  }
}

resource "aws_iam_role_policy_attachment" "attach_sqs_rw_to_ecs" {
  role       = aws_iam_role.ecs.name
  policy_arn = aws_iam_policy.sqs_rw_policy.arn
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

resource "aws_cloudwatch_log_group" "worker" {
  name              = "/aws/ecs/worker"
  retention_in_days = 7
  lifecycle {
    prevent_destroy = false
  }
}

resource "aws_ecs_task_definition" "this" {
  family                   = "pwnwrk"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 3072
  execution_role_arn = aws_iam_role.ecs.arn
  task_role_arn = aws_iam_role.ecs.arn
  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "public.ecr.aws/i0m2p7r6/pwnctl:latest",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_IS_ECS",
          "value": "true"
        },
        {
          "name": "PWNCTL_Worker__MaxTaskTimeout",
          "value": "${var.ecs_task.max_timeout}"
        },
        {
          "name": "PWNCTL_TaskQueue__Name",
          "value": "${aws_sqs_queue.main.name}"
        },
        {
          "name": "PWNCTL_TaskQueue__VisibilityTimeout",
          "value": "${var.sqs_visibility_timeout}"
        },
        {
          "name": "PWNCTL_OutputQueue__Name",
          "value": "${aws_sqs_queue.output.name}"
        },
        {
          "name": "PWNCTL_OutputQueue__VisibilityTimeout",
          "value": "${var.sqs_visibility_timeout}"
        },
        {
          "name": "PWNCTL_Logging__MinLevel",
          "value": "Debug"
        },
        {
          "name": "PWNCTL_Logging__FilePath",
          "value": "${var.efs_mount_point}"
        },
        {
          "name": "PWNCTL_Logging__LogGroup",
          "value": "${aws_cloudwatch_log_group.worker.name}"
        },
        {
          "name": "PWNCTL_Db__Name",
          "value": "${var.rds_postgres_databasename}"
        },
        {
          "name": "PWNCTL_Db__Username",
          "value": "${var.rds_postgres_username}"
        },
        {
          "name": "PWNCTL_Db__Password",
          "value": "${aws_secretsmanager_secret_version.password.secret_string}"
        },
        {
          "name": "PWNCTL_Db__Host",
          "value": "${aws_db_instance.this.endpoint}"
        },
        {
          "name": "PWNCTL_INSTALL_PATH",
          "value": "${var.efs_mount_point}"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "${aws_cloudwatch_log_group.worker.name}",
          "awslogs-region": "${var.region}",
          "awslogs-stream-prefix": "ecs"
        }
      },
      "mountPoints": [
        {
          "sourceVolume": "pwnctl-fs",
          "containerPath": "${var.efs_mount_point}"
        }
      ]
    }
  ]
  DEFINITION

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

resource "aws_ecs_cluster" "this" {
  name = "pwnctl-cluster"
}

resource "aws_ecs_service" "this" {
  name            = var.ecs_service.name
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.this.arn
  desired_count   = 0
  depends_on      = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.this, 
    aws_iam_role.ecs
  ]

  network_configuration {
    subnets = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    weight = 1
  }
}

resource "aws_appautoscaling_target" "this" {
  max_capacity       = var.ecs_task.max_instances
  min_capacity       = var.ecs_service.min_capacity
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.this.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_appautoscaling_policy" "this" {
  name               = "PwnctlStackPwnctlSvcTaskCountTargetScaleOutPolicyLowerPolicy9ECA2AC3"
  policy_type        = "StepScaling"
  resource_id  = aws_appautoscaling_target.this.id
  scalable_dimension = aws_appautoscaling_target.this.scalable_dimension
  service_namespace  = aws_appautoscaling_target.this.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ExactCapacity"
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_upper_bound = 1
      scaling_adjustment = 0
    } 

    dynamic "step_adjustment" {
      for_each = toset([for i in range(0, var.ecs_task.max_instances/3, 1) : i])

      content {
        metric_interval_lower_bound = 10 * step_adjustment.value + 1
        metric_interval_upper_bound = 10 * (step_adjustment.value + 1) + 1
        scaling_adjustment = (step_adjustment.value+1) * 3 + (var.ecs_task.max_instances%3)
      }
    }

    step_adjustment {
      metric_interval_lower_bound = 10 * (var.ecs_task.max_instances/3) + 1
      scaling_adjustment = var.ecs_task.max_instances
    } 
  }
}

resource "aws_cloudwatch_metric_alarm" "task_queue_depth_metric" {
  alarm_name                = "task_queue_depth_metric"
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  threshold = 0
  evaluation_periods        = 1
  insufficient_data_actions = []

  metric_query {
    id          = "visibleMessages"

    metric {
      metric_name = "ApproximateNumberOfMessagesVisible"
      namespace   = "AWS/SQS"
      period      = 60
      stat        = "Maximum"

      dimensions = {
        QueueName = aws_sqs_queue.main.name
      }
    }
  }

  metric_query {
    id          = "inFlightMessages"
    
    metric {
      metric_name = "ApproximateNumberOfMessagesNotVisible"
      namespace   = "AWS/SQS"
      period      = 60
      stat        = "Maximum"

      dimensions = {
        QueueName = aws_sqs_queue.main.name
      }
    }
  }

  metric_query {
    id          = "allMessages"
    expression  = "visibleMessages + inFlightMessages"
    return_data = "true"
  }

  alarm_description = "This metric monitors pending work in the main task queue."
  alarm_actions     = [aws_appautoscaling_policy.this.arn]
}