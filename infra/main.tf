

module "sqs" {
    source = "./modules/sqs"
    stack_name = "pwnctl"
    id         = random_id.nonce
}

module "base" {
    source = "./modules/base"
    stack_name = "pwnctl"
    id         = random_id.nonce
}
