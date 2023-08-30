resource "aws_sqs_queue" "main" {
  name                      = "task-${var.env}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = var.sqs_visibility_timeout
  max_message_size          = 8192
  message_retention_seconds = var.message_retention_seconds
  receive_wait_time_seconds = 3
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 4
  })

  tags = {
    Description = "PwnCtl task queue."
    Env = var.env
  }
}

resource "aws_sqs_queue" "dlq" {
  name                        = "task-dlq-${var.env}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled    = true
  visibility_timeout_seconds = var.sqs_visibility_timeout
  max_message_size           = 8192
  message_retention_seconds = var.message_retention_seconds
  receive_wait_time_seconds  = 20

  tags = {
    Description = "PwnCtl task dead letter queue."
    Env = var.env
  }
}

resource "aws_sqs_queue" "output" {
  name                      = "output-${var.env}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = var.sqs_visibility_timeout
  max_message_size          = 8192
  message_retention_seconds = var.message_retention_seconds
  receive_wait_time_seconds = 3

  tags = {
    Description = "PwnCtl output queue."
    Env = var.env
  }
}

