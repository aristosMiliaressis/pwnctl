resource "aws_vpc" "main" {
  tags = {
    Name = "PwnCtl ${random_id.id.hex} VPC"
  }

  cidr_block = "10.10.0.0/16"
  enable_dns_hostnames = true
}

resource "aws_internet_gateway" "this" {
  vpc_id = aws_vpc.main.id

  tags = {
    Name = "PwnCtl ${random_id.id.hex} Internet Gateway"
  }
}

resource "aws_subnet" "public" {
  for_each = {
    a = "10.10.10.0/24"
    b = "10.10.20.0/24"
  }

  vpc_id = aws_vpc.main.id

  tags = {
    Name = "PwnCtl ${random_id.id.hex} Public Subnet at ${var.region}${each.key}"
  }

  availability_zone = "${var.region}${each.key}"
  cidr_block = each.value
}

resource "aws_subnet" "private" {
  for_each = {
    a = "10.10.30.0/24"
    b = "10.10.40.0/24"
  }

  vpc_id = aws_vpc.main.id

  tags = {
    Name = "PwnCtl ${random_id.id.hex} Private Subnet at ${var.region}${each.key}"
  }

  availability_zone = "${var.region}${each.key}"
  cidr_block = each.value
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.main.id

  tags = {
    Name = "PwnCtl ${random_id.id.hex} Route Table for Public Subnet"
  }

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.this.id
  }
}

resource "aws_route_table" "private" {
  vpc_id = aws_vpc.main.id

  tags = {
    Name = "PwnCtl ${random_id.id.hex} Route Table for Private Subnet"
  }
}

resource "aws_route_table_association" "public" {
  for_each = aws_subnet.public

  subnet_id = each.value.id
  route_table_id = aws_route_table.public.id
}

resource "aws_route_table_association" "private" {
  for_each = aws_subnet.private

  subnet_id = each.value.id
  route_table_id = aws_route_table.private.id
}
