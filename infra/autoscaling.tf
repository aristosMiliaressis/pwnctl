resource "aws_appautoscaling_target" "exec_short" {
  max_capacity       = var.exec_instance_count.shortlived
  min_capacity       = 0
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.exec_short.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_appautoscaling_target" "exec_long" {
  max_capacity       = var.exec_instance_count.longlived
  min_capacity       = 0
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.exec_long.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_appautoscaling_policy" "exec_long" {
  name               = "exec_long"
  policy_type        = "StepScaling"
  resource_id        = aws_appautoscaling_target.exec_long.id
  scalable_dimension = aws_appautoscaling_target.exec_long.scalable_dimension
  service_namespace  = aws_appautoscaling_target.exec_long.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ExactCapacity"
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_upper_bound = 1
      scaling_adjustment          = 0
    }

    dynamic "step_adjustment" {
      for_each = toset([for i in range(0, var.exec_instance_count.longlived / var.exec_step_size.longlived, 1) : i])

      content {
        metric_interval_lower_bound = var.exec_step_interval.longlived * step_adjustment.value + 1
        metric_interval_upper_bound = var.exec_step_interval.longlived * (step_adjustment.value + 1) + 1
        scaling_adjustment          = (step_adjustment.value + 1) * var.exec_step_size.longlived + (var.exec_instance_count.longlived % var.exec_step_size.longlived)
      }
    }

    step_adjustment {
      metric_interval_lower_bound = var.exec_step_interval.longlived * (var.exec_instance_count.longlived / var.exec_step_size.longlived) + 1
      scaling_adjustment          = var.exec_instance_count.longlived
    }
  }
}

resource "aws_appautoscaling_policy" "exec_short" {
  name               = "exec_short"
  policy_type        = "StepScaling"
  resource_id        = aws_appautoscaling_target.exec_short.id
  scalable_dimension = aws_appautoscaling_target.exec_short.scalable_dimension
  service_namespace  = aws_appautoscaling_target.exec_short.service_namespace

  step_scaling_policy_configuration {
    adjustment_type         = "ExactCapacity"
    metric_aggregation_type = "Maximum"

    step_adjustment {
      metric_interval_upper_bound = 1
      scaling_adjustment          = 0
    }

    dynamic "step_adjustment" {
      for_each = toset([for i in range(0, var.exec_instance_count.shortlived / var.exec_step_size.shortlived, 1) : i])

      content {
        metric_interval_lower_bound = var.exec_step_interval.shortlived * step_adjustment.value + 1
        metric_interval_upper_bound = var.exec_step_interval.shortlived * (step_adjustment.value + 1) + 1
        scaling_adjustment          = (step_adjustment.value + 1) * var.exec_step_size.shortlived + (var.exec_instance_count.shortlived % var.exec_step_size.shortlived)
      }
    }

    step_adjustment {
      metric_interval_lower_bound = var.exec_step_interval.shortlived * (var.exec_instance_count.shortlived / var.exec_step_size.shortlived) + 1
      scaling_adjustment          = var.exec_instance_count.shortlived
    }
  }
}

resource "aws_appautoscaling_target" "proc" {
  max_capacity       = 1
  min_capacity       = 0
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.proc.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
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
      scaling_adjustment          = 0
    }

    step_adjustment {
      metric_interval_lower_bound = 1
      scaling_adjustment          = 1
    }
  }
}