output "vpc" {
  value = aws_vpc.main
}

output "public_subnet" {
    value = aws_subnet.public
}

output "private_subnet" {
    value = aws_subnet.private
}

output "exec_image" {
    value = docker_registry_image.exec
}

output "proc_image" {
    value = docker_registry_image.proc
}
