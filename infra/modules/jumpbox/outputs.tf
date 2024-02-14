output "jumpbox_ip" {
    value = aws_instance.this.public_ip
}