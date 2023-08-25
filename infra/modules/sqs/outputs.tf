output "main_queue" {
    value = aws_sqs_queue.main
}

output "output_queue" {
    value = aws_sqs_queue.output
}

output "sqs_rw_policy" {
    value = aws_iam_policy.sqs_readwrite
}

output "sqs_visibility_timeout" {
    value = var.sqs_visibility_timeout
}