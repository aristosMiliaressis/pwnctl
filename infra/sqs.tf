

module "sqs" {
    source  = "./modules/sqs"
    profile = var.profile
    env     = "prod"
    message_retention_seconds = 1209600 # 14 days
}
