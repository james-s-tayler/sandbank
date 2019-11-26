resource "aws_instance" "yolo" {
  ami = "ami-0119aa4d67e59007c"
  instance_type = "t2.nano"
}
