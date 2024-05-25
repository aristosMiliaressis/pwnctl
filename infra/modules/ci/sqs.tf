locals {
  retention_seconds = 180
  visibility_timeout = {
    shortlived = 240
    longlived  = 900
    output     = 450
  } 
}

module "sqs_dev" {
  source                        = "../sqs"
  profile                       = var.profile
  env                           = "dev"
  message_retention_seconds     = local.retention_seconds
  shortlived_visibility_timeout = local.visibility_timeout.shortlived
  longlived_visibility_timeout  = local.visibility_timeout.longlived
  output_visibility_timeout     = local.visibility_timeout.output
}
