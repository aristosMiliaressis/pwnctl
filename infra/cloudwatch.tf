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

resource "aws_cloudwatch_metric_alarm" "longlived_tasks_queue_depth" {
  alarm_name                = "longlived_tasks_queue_depth"
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  threshold                 = 0
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
        QueueName = module.sqs.longlived_tasks_queue.name
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
        QueueName = module.sqs.longlived_tasks_queue.name
      }
    }
  }

  metric_query {
    id          = "allMessages"
    expression  = "visibleMessages + inFlightMessages"
    return_data = "true"
  }

  alarm_description = "This metric monitors pending work in the long lived task queue."
  alarm_actions     = [aws_appautoscaling_policy.exec_long.arn]
}

resource "aws_cloudwatch_metric_alarm" "shortlived_tasks_queue_depth" {
  alarm_name                = "shortlived_tasks_queue_depth"
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  threshold                 = 0
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
        QueueName = module.sqs.shortlived_tasks_queue.name
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
        QueueName = module.sqs.shortlived_tasks_queue.name
      }
    }
  }

  metric_query {
    id          = "allMessages"
    expression  = "visibleMessages + inFlightMessages"
    return_data = "true"
  }

  alarm_description = "This metric monitors pending work in the short lived task queue."
  alarm_actions     = [aws_appautoscaling_policy.exec_short.arn]
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
    id          = "inFlightLongLivedTaskMessages"

    metric {
      metric_name = "ApproximateNumberOfMessagesNotVisible"
      namespace   = "AWS/SQS"
      period      = 60
      stat        = "Maximum"

      dimensions = {
        QueueName = module.sqs.longlived_tasks_queue.name
      }
    }
  }

  metric_query {
    id          = "inFlightShortLivedTaskMessages"

    metric {
      metric_name = "ApproximateNumberOfMessagesNotVisible"
      namespace   = "AWS/SQS"
      period      = 60
      stat        = "Maximum"

      dimensions = {
        QueueName = module.sqs.shortlived_tasks_queue.name
      }
    }
  }

  metric_query {
    id          = "allMessages"
    expression  = "visibleOutputMessages + inFlightOutputMessages + inFlightLongLivedTaskMessages + inFlightShortLivedTaskMessages"
    return_data = "true"
  }

  alarm_description = "This metric monitors unprocessed batches in the output queue."
  alarm_actions     = [aws_appautoscaling_policy.proc.arn]
}