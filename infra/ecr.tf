data "aws_ecr_repository" "exec_short" {
  name = "pwnctl-exec-short"
}

data "aws_ecr_repository" "exec_long" {
  name = "pwnctl-exec-long"
}

data "aws_ecr_repository" "proc" {
  name = "pwnctl-proc"
}

resource "docker_registry_image" "exec_short" {
  name          = "${data.aws_ecr_repository.exec_short.repository_url}:${data.external.commit_hash.result.sha}"
  keep_remotely = true
}

resource "docker_registry_image" "exec_long" {
  name          = "${data.aws_ecr_repository.exec_long.repository_url}:${data.external.commit_hash.result.sha}"
  keep_remotely = true
}

resource "docker_registry_image" "proc" {
  name          = "${data.aws_ecr_repository.proc.repository_url}:${data.external.commit_hash.result.sha}"
  keep_remotely = true
}
