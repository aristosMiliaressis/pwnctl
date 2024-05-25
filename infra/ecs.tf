resource "aws_ecs_cluster" "this" {
  name = "pwnctl_cluster"
}

resource "aws_ecs_task_definition" "exec_short" {
  family                   = "pwnctl-exec-short"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 256
  memory                   = 512
  execution_role_arn       = data.aws_iam_role.ecs_service.arn
  task_role_arn            = data.aws_iam_role.ecs_service.arn

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${docker_registry_image.exec_short.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_COMMIT_HASH",
          "value": "${data.external.commit_hash.result.sha}"
        },
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
          "value": "${var.db_name}"
        },
        {
          "name": "PWNCTL_Db__Username",
          "value": "${var.db_user}"
        },
        {
          "name": "PWNCTL_Db__Host",
          "value": "${var.db_host}"
        },
        {
          "name": "PWNCTL_IS_PROD",
          "value": "true"
        },
        {
          "name": "PWNCTL_FS_MOUNT_POINT",
          "value": "${local.efs_mount_point}"
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
          "containerPath": "${local.efs_mount_point}"
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
    data.aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets          = [var.public_subnet_a, var.public_subnet_b]
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
  memory                   = 1024
  execution_role_arn       = data.aws_iam_role.ecs_service.arn
  task_role_arn            = data.aws_iam_role.ecs_service.arn

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${docker_registry_image.exec_long.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_COMMIT_HASH",
          "value": "${data.external.commit_hash.result.sha}"
        },
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
          "value": "${var.db_name}"
        },
        {
          "name": "PWNCTL_Db__Username",
          "value": "${var.db_user}"
        },
        {
          "name": "PWNCTL_Db__Host",
          "value": "${var.db_host}"
        },
        {
          "name": "PWNCTL_IS_PROD",
          "value": "true"
        },
        {
          "name": "PWNCTL_FS_MOUNT_POINT",
          "value": "${local.efs_mount_point}"
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
          "containerPath": "${local.efs_mount_point}"
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
    data.aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets          = [var.public_subnet_a, var.public_subnet_b]
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

data "aws_iam_role" "ecs_service" {
  name = "pwnctl-ecs-service"
}

resource "aws_ecs_task_definition" "proc" {
  family                   = "pwnctl-proc"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 1024
  execution_role_arn       = data.aws_iam_role.ecs_service.arn
  task_role_arn            = data.aws_iam_role.ecs_service.arn

  container_definitions = <<DEFINITION
  [
    {
      "name": "pwnctl",
      "image": "${docker_registry_image.proc.name}",
      "essential": true,
      "stopTimeout": 120,
      "environment": [
        {
          "name": "PWNCTL_COMMIT_HASH",
          "value": "${data.external.commit_hash.result.sha}"
        },
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
          "value": "${var.db_name}"
        },
        {
          "name": "PWNCTL_Db__Username",
          "value": "${var.db_user}"
        },
        {
          "name": "PWNCTL_Db__Host",
          "value": "${var.db_host}"
        },
        {
          "name": "PWNCTL_IS_PROD",
          "value": "true"
        },
        {
          "name": "PWNCTL_FS_MOUNT_POINT",
          "value": "${local.efs_mount_point}"
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
          "containerPath": "${local.efs_mount_point}"
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
    aws_ecs_task_definition.proc,
    data.aws_iam_role.ecs_service
  ]

  network_configuration {
    subnets          = [var.public_subnet_a, var.public_subnet_b]
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
