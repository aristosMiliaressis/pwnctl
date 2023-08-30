

module "sqs" {
    source  = "./modules/sqs"
    profile = var.profile
    env     = "prod"
}
