output "db_host" {
  value = aws_db_instance.this.endpoint
}

output "db_name" {
  value = local.db_name
}

output "db_user" {
  value = local.db_user
}