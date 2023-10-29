
data "aws_ecr_repository" "exec" {
  name                 = "pwnctl-exec"

  tags = {
    Description = "Task executor image repository."
  }
}

resource "docker_registry_image" "exec" {
  name = "${data.aws_ecr_repository.exec.repository_url}:${data.external.commit_hash.result.sha}"
  keep_remotely = true
}

data "aws_ecr_repository" "proc" {
  name                 = "pwnctl-proc"

  tags = {
    Description = "output processor image repository."
  }
}

resource "docker_registry_image" "proc" {
  name = "${data.aws_ecr_repository.proc.repository_url}:${data.external.commit_hash.result.sha}"
  keep_remotely = true
}
