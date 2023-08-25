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

data "aws_ecr_repository" "exec" {
  name                 = "${var.stack_name}-exec"
}

resource "docker_registry_image" "exec" {
  name = "${data.aws_ecr_repository.exec.repository_url}:latest"
  keep_remotely = true
}

data "aws_ecr_repository" "proc" {
  name                 = "${var.stack_name}-proc"
}

resource "docker_registry_image" "proc" {
  name = "${data.aws_ecr_repository.proc.repository_url}:latest"
  keep_remotely = true
}
