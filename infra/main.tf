

module "sqs" {
    source  = "./modules/sqs"
    profile = var.profile
    nonce   = random_id.nonce.hex
}

module "base" {
    source     = "./modules/base"
    stack_name = "pwnctl"
    nonce      = random_id.nonce.hex
}
