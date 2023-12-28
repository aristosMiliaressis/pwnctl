
module "sqs" {
  source  = "../sqs"
  profile = var.profile
  env     = "dev"
  message_retention_seconds = 180
  shortlived_visibility_timeout = 240
  longlived_visibility_timeout  = 900
  output_visibility_timeout     = 450
}