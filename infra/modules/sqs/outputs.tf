output "main_queue" {
    value = aws_sqs_queue.main
}

output "output_queue" {
    value = aws_sqs_queue.output
}

output "sqs_visibility_timeout" {
    value = var.sqs_visibility_timeout
}