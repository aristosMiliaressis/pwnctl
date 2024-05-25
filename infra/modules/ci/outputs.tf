output "vpc_id" {
    value = aws_vpc.this.id
}

output "public_subnet_a" {
    value = aws_subnet.public["a"].id
}

output "public_subnet_b" {
    value = aws_subnet.public["b"].id
}

output "private_subnet_a" {
    value = aws_subnet.private["a"].id
}

output "private_subnet_b" {
    value = aws_subnet.private["b"].id
}