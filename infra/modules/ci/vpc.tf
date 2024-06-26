resource "aws_vpc" "this" {
  tags = {
    Name = "PwnCtl VPC"
  }

  cidr_block                       = "10.10.0.0/16"
  assign_generated_ipv6_cidr_block = true
  enable_dns_hostnames             = true
}

resource "aws_internet_gateway" "this" {
  vpc_id = aws_vpc.this.id

  tags = {
    Name = "PwnCtl Internet Gateway"
  }

  depends_on = [
    aws_subnet.private
  ]
}

resource "aws_subnet" "public" {
  for_each = {
    a = 1
    b = 2
  }

  vpc_id = aws_vpc.this.id

  tags = {
    Name = "PwnCtl Public Subnet ${each.key}"
  }

  availability_zone = "${data.external.aws_region.result.region}${each.key}"
  cidr_block        = cidrsubnet(aws_vpc.this.cidr_block, 4, each.value)
  ipv6_cidr_block   = cidrsubnet(aws_vpc.this.ipv6_cidr_block, 4, each.value)
}

resource "aws_subnet" "private" {
  for_each = {
    a = 3
    b = 4
  }

  vpc_id = aws_vpc.this.id

  tags = {
    Name = "PwnCtl Private Subnet at ${data.external.aws_region.result.region}${each.key}"
  }

  availability_zone = "${data.external.aws_region.result.region}${each.key}"
  cidr_block        = cidrsubnet(aws_vpc.this.cidr_block, 4, each.value)
  ipv6_cidr_block   = cidrsubnet(aws_vpc.this.ipv6_cidr_block, 4, each.value)
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.this.id

  tags = {
    Name = "PwnCtl Route Table for Public Subnet"
  }

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.this.id
  }

  route {
    ipv6_cidr_block = "::/0"
    gateway_id      = aws_internet_gateway.this.id
  }
}

resource "aws_route_table" "private" {
  vpc_id = aws_vpc.this.id

  tags = {
    Name = "PwnCtl Route Table for Private Subnet"
  }
}

resource "aws_route_table_association" "public" {
  for_each = aws_subnet.public

  subnet_id      = each.value.id
  route_table_id = aws_route_table.public.id
}

resource "aws_route_table_association" "private" {
  for_each = aws_subnet.private

  subnet_id      = each.value.id
  route_table_id = aws_route_table.private.id
}
