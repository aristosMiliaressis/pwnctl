data "external" "current_repo_name" {
  program = ["bash", "-c", "git remote -v | grep push | awk '{print $2}' | cut -d '/' -f4- | jq --raw-input '. | { repo_name: (.) }'"]
}

/* module "oidc_github" {
  source  = "unfunco/oidc-github/aws"
  version = "1.5.0"

  github_repositories = [
    "${ data.external.current_repo_name.result.repo_name }"
  ]
} */

resource "aws_ecr_repository" "exec" {
  name                 = "${var.stack_name}-exec"
  image_tag_mutability = "MUTABLE"
}

/* resource "docker_registry_image" "exec" {
  name = "${aws_ecr_repository.exec.repository_url}:latest"
  keep_remotely = true

  depends_on = [
    docker_image.exec
  ]
} */

/* resource "docker_image" "exec" {
  name         = "${aws_ecr_repository.exec.repository_url}:latest"
  keep_locally = true

  build {
    context = "../src/core"
    dockerfile = "pwnctl.exec/Dockerfile"
  }

  triggers = {
    dir_sha1 = sha1(join("", [for f in fileset(path.module, "../src/core/*") : filesha1(f)]))
  }
} */

resource "aws_ecr_repository" "proc" {
  name                 = "${var.stack_name}-proc"
  image_tag_mutability = "MUTABLE"
}

/* resource "docker_registry_image" "proc" {
  name = "${aws_ecr_repository.proc.repository_url}:latest"
  keep_remotely = true

  depends_on = [
    docker_image.proc
  ]
} */

/* resource "docker_image" "proc" {
  name         = "${aws_ecr_repository.proc.repository_url}:latest"
  keep_locally = true

  build {
    context = "../src/core"
    dockerfile = "pwnctl.proc/Dockerfile"
  }

  triggers = {
    dir_sha1 = sha1(join("", [for f in fileset(path.module, "../src/core/*") : filesha1(f)]))
  }
} */