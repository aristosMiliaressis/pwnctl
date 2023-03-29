resource "aws_sqs_queue" "main" {
  name                      = "pwnctl_${random_id.id.hex}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = var.sqs_visibility_timeout
  max_message_size          = 8192
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 20
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 3
  })

  tags = {
    Name = "pwnctl_queue"
  }
}

resource "aws_sqs_queue" "dlq" {
  name                      = "pwnctl_${random_id.id.hex}_dlq.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = var.sqs_visibility_timeout
  max_message_size          = 8192
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 20

  tags = {
    Name = "pwnctl_dlq"
  }
}

resource "aws_ssm_parameter" "queue_name" {
  name  = "/pwnctl/TaskQueue/Name"
  type  = "String"
  value = aws_sqs_queue.main.name
}

output "apiqueue_name_url" {
  value = aws_sqs_queue.main.name
}
