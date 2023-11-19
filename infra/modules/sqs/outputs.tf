output "shortlived_tasks_queue" {
  value = aws_sqs_queue.shortlived-tasks
}

output "longlived_tasks_queue" {
  value = aws_sqs_queue.longlived-tasks
}

output "output_queue" {
  value = aws_sqs_queue.output
}

output "shortlived_visibility_timeout" {
  value = var.shortlived_visibility_timeout
}

output "longlived_visibility_timeout" {
  value = var.longlived_visibility_timeout
}

output "output_visibility_timeout" {
  value = var.output_visibility_timeout
}