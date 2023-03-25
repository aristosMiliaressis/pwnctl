resource "aws_sqs_queue" "this" {
  name                      = "pwnctl.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = 600
  max_message_size          = 8192
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 20
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 3
  })

  tags = {
    Name = "pwnctl_${var.pwnctl_id}_queue"
  }
}

resource "aws_sqs_queue" "dlq" {
  name                      = "pwnctl_dlq.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = 600
  max_message_size          = 8192
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 20

  tags = {
    Name = "pwnctl_${var.pwnctl_id}_dlq"
  }
}