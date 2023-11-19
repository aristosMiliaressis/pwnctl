
module "sqs" {
  source  = "../sqs"
  profile = var.profile
  env     = "dev"
  message_retention_seconds = 1800
  shortlived_visibility_timeout = 180
  longlived_visibility_timeout  = 900
  output_visibility_timeout     = 450
}