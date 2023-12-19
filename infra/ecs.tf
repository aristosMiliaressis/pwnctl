resource "aws_ecs_cluster" "this" {
  name = "pwnctl_cluster"
}

resource "aws_ecs_task_definition" "exec_short" {
  family                   = "pwnctl-exec-short"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 256
  memory                   = 1024
  execution_role_arn       = aws_iam_role.ecs_service.arn
  task_role_arn            = aws_iam_role.ecs_service.arn

  depends_on = [
    aws_db_instance.this
  ]

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${docker_registry_image.exec_short.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_Worker__MaxTaskTimeout",
          "value": "${module.sqs.shortlived_visibility_timeout}"
        },
        {
          "name": "PWNCTL_ShortLivedTaskQueue__Name",
          "value": "${module.sqs.shortlived_tasks_queue.name}"
        },
        {
          "name": "PWNCTL_OutputQueue__Name",
          "value": "${module.sqs.output_queue.name}"
        },
        {
          "name": "PWNCTL_Logging__MinLevel",
          "value": "Information"
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
          "name": "PWNCTL_IS_PROD",
          "value": "true"
        },
        {
          "name": "PWNCTL_FS_MOUNT_POINT",
          "value": "${var.efs_mount_point}"
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
    name = "pwnctl-fs"
    efs_volume_configuration {
      file_system_id     = aws_efs_file_system.this.id
      root_directory     = "/"
      transit_encryption = "ENABLED"
      authorization_config {
        iam             = "ENABLED"
        access_point_id = aws_efs_access_point.this.id
      }
    }
  }
}

resource "aws_ecs_service" "exec_short" {
  name            = "pwnctl_exec_short"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.exec_short.arn
  desired_count   = 0
  depends_on = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.exec_short,
    aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets          = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE_SPOT"
    weight            = 1
  }

  lifecycle {
    ignore_changes = [desired_count]
  }
}

resource "aws_ecs_task_definition" "exec_long" {
  family                   = "pwnctl-exec-long"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 2048
  execution_role_arn       = aws_iam_role.ecs_service.arn
  task_role_arn            = aws_iam_role.ecs_service.arn

  depends_on = [
    aws_db_instance.this
  ]

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${docker_registry_image.exec_long.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_Worker__MaxTaskTimeout",
          "value": "${var.task_timeout}"
        },
        {
          "name": "PWNCTL_LongLivedTaskQueue__Name",
          "value": "${module.sqs.longlived_tasks_queue.name}"
        },
        {
          "name": "PWNCTL_LongLivedTaskQueue__VisibilityTimeout",
          "value": "${module.sqs.longlived_visibility_timeout}"
        },
        {
          "name": "PWNCTL_OutputQueue__Name",
          "value": "${module.sqs.output_queue.name}"
        },
        {
          "name": "PWNCTL_Logging__MinLevel",
          "value": "Information"
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
          "name": "PWNCTL_IS_PROD",
          "value": "true"
        },
        {
          "name": "PWNCTL_FS_MOUNT_POINT",
          "value": "${var.efs_mount_point}"
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
    name = "pwnctl-fs"
    efs_volume_configuration {
      file_system_id     = aws_efs_file_system.this.id
      root_directory     = "/"
      transit_encryption = "ENABLED"
      authorization_config {
        iam             = "ENABLED"
        access_point_id = aws_efs_access_point.this.id
      }
    }
  }
}

resource "aws_ecs_service" "exec_long" {
  name            = "pwnctl_exec_long"
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.exec_long.arn
  desired_count   = 0
  depends_on = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.exec_long,
    aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets          = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    weight            = 1
  }

  lifecycle {
    ignore_changes = [desired_count]
  }
}


resource "aws_ecs_task_definition" "proc" {
  family                   = "pwnctl-proc"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 1024
  memory                   = 3072
  execution_role_arn       = aws_iam_role.ecs_service.arn
  task_role_arn            = aws_iam_role.ecs_service.arn

  depends_on = [
    aws_db_instance.this,
    aws_security_group.allow_postgres
  ]

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${docker_registry_image.proc.name}",
      "essential": true,
      "stopTimeout": 30,
      "environment": [
        {
          "name": "PWNCTL_LongLivedTaskQueue__Name",
          "value": "${module.sqs.longlived_tasks_queue.name}"
        },
        {
          "name": "PWNCTL_ShortLivedTaskQueue__Name",
          "value": "${module.sqs.shortlived_tasks_queue.name}"
        },
        {
          "name": "PWNCTL_OutputQueue__Name",
          "value": "${module.sqs.output_queue.name}"
        },
        {
          "name": "PWNCTL_Logging__MinLevel",
          "value": "Information"
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
          "name": "PWNCTL_IS_PROD",
          "value": "true"
        },
        {
          "name": "PWNCTL_FS_MOUNT_POINT",
          "value": "${var.efs_mount_point}"
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
    name = "pwnctl-fs"
    efs_volume_configuration {
      file_system_id     = aws_efs_file_system.this.id
      root_directory     = "/"
      transit_encryption = "ENABLED"
      authorization_config {
        iam             = "ENABLED"
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
  depends_on = [
    aws_ecs_cluster.this,
    aws_ecs_task_definition.proc,
    aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets          = [for k, v in aws_subnet.public : aws_subnet.public[k].id]
    assign_public_ip = "true"
  }

  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    weight            = 1
  }

  lifecycle {
    ignore_changes = [desired_count]
  }
}
