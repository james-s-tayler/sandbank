terraform {
  required_providers {
    aws = "~> 2.39"
  }
}

provider "aws" {
  region = var.region
}
