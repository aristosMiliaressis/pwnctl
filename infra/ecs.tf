
data "aws_ecr_repository" "exec" {
  name                 = "pwnctl-exec"

  tags = {
    Description = "Task executor image repository."
  }
}

data "docker_registry_image" "proc" {
  name = "${data.aws_ecr_repository.proc.repository_url}:latest"
}

data "aws_ecr_repository" "proc" {
  name                 = "pwnctl-proc"

  tags = {
    Description = "Task processor image repository."
  }
}

data "docker_registry_image" "exec" {
  name = "${data.aws_ecr_repository.exec.repository_url}:latest"
}

resource "aws_ecs_cluster" "this" {
  name = var.ecs_cluster.name
}

resource "aws_cloudwatch_log_group" "exec" {
  name              = "/aws/ecs/exec"
  retention_in_days = 1
  lifecycle {
    prevent_destroy = false
  }
}

resource "aws_cloudwatch_log_group" "proc" {
  name              = "/aws/ecs/proc"
  retention_in_days = 1
  lifecycle {
    prevent_destroy = false
  }
}

resource "aws_ecs_task_definition" "exec" {
  family                   = "pwnctl-exec"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 2048
  execution_role_arn = aws_iam_role.ecs_service.arn
  task_role_arn = aws_iam_role.ecs_service.arn

  depends_on = [
    aws_db_instance.this
  ]

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${data.docker_registry_image.exec.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_IMAGE_HASH",
          "value": "${data.docker_registry_image.exec.sha256_digest}"
        },
        {
          "name": "PWNCTL_Worker__MaxTaskTimeout",
          "value": "${var.task_timeout}"
        },
        {
          "name": "PWNCTL_TaskQueue__Name",
          "value": "${module.sqs.main_queue.name}"
        },
        {
          "name": "PWNCTL_TaskQueue__VisibilityTimeout",
          "value": "${module.sqs.sqs_visibility_timeout}"
        },
        {
          "name": "PWNCTL_OutputQueue__Name",
          "value": "${module.sqs.output_queue.name}"
        },
        {
          "name": "PWNCTL_OutputQueue__VisibilityTimeout",
          "value": "${module.sqs.sqs_visibility_timeout}"
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
          "value": "${aws_cloudwatch_log_group.exec.name}"
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
          "name": "PWNCTL_Db__Host",
          "value": "${aws_db_instance.this.endpoint}"
        },
        {
          "name": "PWNCTL_INSTALL_PATH",
          "value": "${var.efs_mount_point}"
        },
        {
          "name": "PWNCTL_IN_VPC",
          "value": "true"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "${aws_cloudwatch_log_group.exec.name}",
          "awslogs-region": "${data.external.aws_region.result.region}",
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

resource "aws_ecs_service" "exec" {
  name            = "pwnctl_exec"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.exec.arn
  desired_count   = 0
  depends_on      = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.exec,
    aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    weight = 1
  }

  lifecycle {
    ignore_changes = [ desired_count ]
  }
}

resource "aws_appautoscaling_target" "exec" {
  max_capacity       = var.ecs_service.max_capacity
  min_capacity       = var.ecs_service.min_capacity
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.exec.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_cloudwatch_metric_alarm" "task_queue_depth" {
  alarm_name                = "task-queue-depth"
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
        QueueName = module.sqs.main_queue.name
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
        QueueName = module.sqs.main_queue.name
      }
    }
  }

  metric_query {
    id          = "allMessages"
    expression  = "visibleMessages + inFlightMessages"
    return_data = "true"
  }

  alarm_description = "This metric monitors pending work in the main task queue."
  alarm_actions     = [aws_appautoscaling_policy.exec.arn]
}

resource "aws_appautoscaling_policy" "exec" {
  name               = "exec"
  policy_type        = "StepScaling"
  resource_id  = aws_appautoscaling_target.exec.id
  scalable_dimension = aws_appautoscaling_target.exec.scalable_dimension
  service_namespace  = aws_appautoscaling_target.exec.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ExactCapacity"
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_upper_bound = 1
      scaling_adjustment = 0
    }

    dynamic "step_adjustment" {
      for_each = toset([for i in range(0, var.ecs_service.max_capacity/3, 1) : i])

      content {
        metric_interval_lower_bound = 10 * step_adjustment.value + 1
        metric_interval_upper_bound = 10 * (step_adjustment.value + 1) + 1
        scaling_adjustment = (step_adjustment.value+1) * 3 + (var.ecs_service.max_capacity%3)
      }
    }

    step_adjustment {
      metric_interval_lower_bound = 10 * (var.ecs_service.max_capacity/3) + 1
      scaling_adjustment = var.ecs_service.max_capacity
    }
  }
}

resource "aws_ecs_task_definition" "proc" {
  family                   = "pwnctl-proc"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 1024
  memory                   = 4096
  execution_role_arn = aws_iam_role.ecs_service.arn
  task_role_arn = aws_iam_role.ecs_service.arn

  depends_on = [
    aws_db_instance.this,
    aws_security_group.allow_postgres
  ]

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${data.docker_registry_image.proc.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_IMAGE_HASH",
          "value": "${data.docker_registry_image.proc.sha256_digest}"
        },
        {
          "name": "PWNCTL_TaskQueue__Name",
          "value": "${module.sqs.main_queue.name}"
        },
        {
          "name": "PWNCTL_TaskQueue__VisibilityTimeout",
          "value": "${module.sqs.sqs_visibility_timeout}"
        },
        {
          "name": "PWNCTL_OutputQueue__Name",
          "value": "${module.sqs.output_queue.name}"
        },
        {
          "name": "PWNCTL_OutputQueue__VisibilityTimeout",
          "value": "${module.sqs.sqs_visibility_timeout}"
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
          "value": "${aws_cloudwatch_log_group.proc.name}"
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
          "name": "PWNCTL_Db__Host",
          "value": "${aws_db_instance.this.endpoint}"
        },
        {
          "name": "PWNCTL_INSTALL_PATH",
          "value": "${var.efs_mount_point}"
        },
        {
          "name": "PWNCTL_IN_VPC",
          "value": "true"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "${aws_cloudwatch_log_group.proc.name}",
          "awslogs-region": "${data.external.aws_region.result.region}",
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

resource "aws_ecs_service" "proc" {
  name            = "pwnctl_proc"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.proc.arn
  desired_count   = 0
  depends_on      = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.proc,
    aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    weight = 1
  }

  lifecycle {
    ignore_changes = [ desired_count ]
  }
}

resource "aws_appautoscaling_target" "proc" {
  max_capacity       = 1
  min_capacity       = 0
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.proc.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_cloudwatch_metric_alarm" "output_queue_depth" {
  alarm_name                = "output-queue-depth"
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  threshold = 0
  evaluation_periods        = 1
  insufficient_data_actions = []

  metric_query {
    id          = "visibleOutputMessages"

    metric {
      metric_name = "ApproximateNumberOfMessagesVisible"
      namespace   = "AWS/SQS"
      period      = 60
      stat        = "Maximum"

      dimensions = {
        QueueName = module.sqs.output_queue.name
      }
    }
  }

  metric_query {
    id          = "inFlightOutputMessages"

    metric {
      metric_name = "ApproximateNumberOfMessagesNotVisible"
      namespace   = "AWS/SQS"
      period      = 60
      stat        = "Maximum"

      dimensions = {
        QueueName = module.sqs.output_queue.name
      }
    }
  }

  metric_query {
    id          = "allMessages"
    expression  = "visibleOutputMessages + inFlightOutputMessages"
    return_data = "true"
  }

  alarm_description = "This metric monitors unprocessed batches in the output queue."
  alarm_actions     = [aws_appautoscaling_policy.proc.arn]
}


resource "aws_appautoscaling_policy" "proc" {
  name               = "pwnctl-proc"
  policy_type        = "StepScaling"
  resource_id        = aws_appautoscaling_target.proc.id
  scalable_dimension = aws_appautoscaling_target.proc.scalable_dimension
  service_namespace  = aws_appautoscaling_target.proc.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ExactCapacity"
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_upper_bound = 1
      scaling_adjustment = 0
    }

    step_adjustment {
      metric_interval_lower_bound = 1
      scaling_adjustment = 1
    }
  }
}