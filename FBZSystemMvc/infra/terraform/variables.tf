variable "region" {
  type    = string
  default = "us-east-1"
}

variable "app_name" {
  type    = string
  default = "fbzsystemmvc"
}

variable "env_name" {
  type    = string
  default = "fbzsystemmvc-env"
}

variable "solution_stack_name" {
  type    = string
  default = "64bit Amazon Linux 2023 v3.8.0 running .NET 9"
}