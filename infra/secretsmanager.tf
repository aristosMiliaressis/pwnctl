resource "random_password" "db" {
  length           = 16
  special          = false
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

resource "aws_secretsmanager_secret" "db_password" {
  name = "/aws/secret/pwnctl/Db/Password"

  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "db_password" {
  secret_id     = aws_secretsmanager_secret.db_password.id
  secret_string = random_password.db.result
}

resource "random_password" "admin" {
  length  = 16
  special = false
}

resource "aws_secretsmanager_secret" "admin_password" {
  name = "/aws/secret/pwnctl/admin_password"

  recovery_window_in_days = 0

  tags = {
    Description = "Admin password used to create the default admin API user."
  }
}

resource "aws_secretsmanager_secret_version" "admin_password" {
  secret_id     = aws_secretsmanager_secret.admin_password.id
  secret_string = random_password.admin.result
}


resource "random_password" "hmac_secret" {
  length  = 32
  special = false
}

resource "aws_secretsmanager_secret" "hmac_secret" {
  name = "/aws/secret/pwnctl/Api/HMACSecret"

  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "hmac_secret" {
  secret_id     = aws_secretsmanager_secret.hmac_secret.id
  secret_string = random_password.hmac_secret.result
}
