resource "aws_sqs_queue" "shortlived-tasks" {
  name                        = "${var.env}-task-shortlived.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled     = true
  visibility_timeout_seconds  = var.shortlived_visibility_timeout
  max_message_size            = 8192
  message_retention_seconds   = var.message_retention_seconds
  receive_wait_time_seconds   = 3
  redrive_policy              = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 4
  })

  tags = {
    Description = "PwnCtl Short Lived task queue."
    Env = var.env
  }
}

resource "aws_sqs_queue" "longlived-tasks" {
  name                        = "${var.env}-task-longlived.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled     = true
  visibility_timeout_seconds  = var.longlived_visibility_timeout
  max_message_size            = 8192
  message_retention_seconds   = var.message_retention_seconds
  receive_wait_time_seconds   = 3
  redrive_policy              = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 4
  })

  tags = {
    Description = "PwnCtl Long Lived task queue."
    Env = var.env
  }
}

resource "aws_sqs_queue" "dlq" {
  name                        = "${var.env}-task-dlq.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled     = true
  visibility_timeout_seconds  = var.longlived_visibility_timeout
  max_message_size            = 8192
  message_retention_seconds   = var.message_retention_seconds
  receive_wait_time_seconds   = 20

  tags = {
    Description = "PwnCtl task dead letter queue."
    Env = var.env
  }
}

resource "aws_sqs_queue" "output" {
  name                        = "${var.env}-output.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled     = true
  visibility_timeout_seconds  = var.output_visibility_timeout
  max_message_size            = 8192
  message_retention_seconds   = var.message_retention_seconds
  receive_wait_time_seconds   = 3

  tags = {
    Description = "PwnCtl output queue."
    Env = var.env
  }
}

