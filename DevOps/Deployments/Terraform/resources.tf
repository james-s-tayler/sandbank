resource "aws_instance" "yolo" {
  ami = var.amazon_linux_2_ami
  instance_type = var.ec2_instance_type
}
