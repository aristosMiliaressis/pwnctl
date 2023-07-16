resource "aws_sqs_queue" "main" {
  name                      = "pwnctl_${random_id.nonce.hex}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = var.sqs_visibility_timeout
  max_message_size          = 8192
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 3
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.dlq.arn
    maxReceiveCount     = 3
  })

  tags = {
    Name = "pwnctl_queue"
  }
}

resource "aws_sqs_queue" "dlq" {
  name                      = "pwnctl_${random_id.nonce.hex}_dlq.fifo"
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

resource "aws_sqs_queue" "output" {
  name                      = "pwnctl_output_${random_id.nonce.hex}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  sqs_managed_sse_enabled = true
  visibility_timeout_seconds  = var.sqs_visibility_timeout
  max_message_size          = 8192
  message_retention_seconds = 1209600 # 14 days
  receive_wait_time_seconds = 3

  tags = {
    Name = "pwnctl_output_queue"
  }
}


data "aws_iam_policy_document" "sqs_readwrite" {
  statement {
    effect = "Allow"

    actions = [
      "sqs:ChangeMessageVisibility",
      "sqs:DeleteMessage",
      "sqs:GetQueueAttributes",
      "sqs:GetQueueUrl",
      "sqs:ReceiveMessage",
      "sqs:SendMessage"
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "sqs_readwrite" {
  name        = "sqs_readwrite"
  path        = "/"
  description = "IAM policy for sqs Read/Write access"
  policy      = data.aws_iam_policy_document.sqs_readwrite.json
}
