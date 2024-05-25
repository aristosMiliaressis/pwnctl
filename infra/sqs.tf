locals {
  retention_seconds = 1209600 # 14 days
  visibility_timeout = {
    shortlived = 240
    longlived  = 900
    output     = 900
  }
}

module "sqs" {
  source                        = "./modules/sqs"
  profile                       = var.profile
  env                           = "prod"
  message_retention_seconds     = local.retention_seconds
  shortlived_visibility_timeout = local.visibility_timeout.shortlived
  longlived_visibility_timeout  = local.visibility_timeout.longlived
  output_visibility_timeout     = local.visibility_timeout.output
}
