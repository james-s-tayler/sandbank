resource "aws_vpc" "vpc" {
  cidr_block = var.network_address_space
  enable_dns_hostnames = "true"
}

resource "aws_internet_gateway" "igw" {
    vpc_id = aws_vpc.vpc.id
}

resource "aws_subnet" "subnet1" {
    cidr_block = var.subnet1_address_space
    map_public_ip_on_launch = "true"
    vpc_id = aws_vpc.vpc.id
    availability_zone = data.availability_zones.available.names[0]
}

resource "aws_route_table" "rtb" {
    vpc_id = aws_vpc.vpc.id

    route {
        cidr_block = "0.0.0.0/0"
        gateway_id = aws_internet_gateway.igw.id
    }
}

resource "aws_route_table_association" "rtba" {
    subnet_id = aws_subnet.subnet1.id
    route_table_id = aws_route_table.rtb.id
}
