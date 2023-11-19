
module "sqs" {
    source  = "./modules/sqs"
    profile = var.profile
    env     = "prod"
    message_retention_seconds     = var.message_retention_seconds
    shortlived_visibility_timeout = var.message_visibility_timeout.shortlived
    longlived_visibility_timeout  = var.message_visibility_timeout.longlived
    output_visibility_timeout     = var.message_visibility_timeout.output
}
