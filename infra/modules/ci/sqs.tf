
module "sqs" {
  source  = "../sqs"
  profile = var.profile
  env     = "dev"
  message_retention_seconds = 1800
}