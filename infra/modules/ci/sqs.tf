
module "sqs" {
  source  = "../sqs"
  profile = var.profile
  env     = "dev"
}