data "aws_ecr_repository" "exec_short" {
  name = "pwnctl-exec-short"
}

data "aws_ecr_repository" "exec_long" {
  name = "pwnctl-exec-long"
}

data "aws_ecr_repository" "proc" {
  name = "pwnctl-proc"
}

data "external" "proc-latest-tag" {
  program = ["bash", "-c", "docker images | grep ${data.aws_ecr_repository.proc.repository_url} | head -n 1 | awk '{print $2}'| jq --raw-input '. | { tag: (.) }'"]
}

data "external" "exec-short-latest-tag" {
  program = ["bash", "-c", "docker images | grep ${data.aws_ecr_repository.proc.repository_url} | head -n 1 | awk '{print $2}'| jq --raw-input '. | { tag: (.) }'"]
}

data "external" "exec-long-latest-tag" {
  program = ["bash", "-c", "docker images | grep ${data.aws_ecr_repository.proc.repository_url} | head -n 1 | awk '{print $2}'| jq --raw-input '. | { tag: (.) }'"]
}

resource "docker_registry_image" "exec_short" {
  name          = "${data.aws_ecr_repository.exec_short.repository_url}:${data.external.exec-short-latest-tag.result.tag}"
  keep_remotely = true
}

resource "docker_registry_image" "exec_long" {
  name          = "${data.aws_ecr_repository.exec_long.repository_url}:${data.external.exec-long-latest-tag.result.tag}"
  keep_remotely = true
}

resource "docker_registry_image" "proc" {
  name          = "${data.aws_ecr_repository.proc.repository_url}:${data.external.proc-latest-tag.result.tag}"
  keep_remotely = true
}
