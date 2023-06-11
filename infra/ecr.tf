data "external" "current_repo_name" {
  program = ["bash", "-c", "git remote -v | grep push | awk '{print $2}' | cut -d '/' -f4- | jq --raw-input '. | { repo_name: (.) }'"]
}

module "oidc_github" {
  source  = "unfunco/oidc-github/aws"
  version = "1.5.0"

  github_repositories = [
    "${ data.external.current_repo_name.result.repo_name }"
  ]
}

resource "aws_ecr_repository" "main" {
  name                 = var.stack_name
  image_tag_mutability = "MUTABLE"
}

resource "docker_registry_image" "pwnctl" {
  name = "${aws_ecr_repository.main.repository_url}:latest"
  keep_remotely = true

  depends_on = [
    docker_image.pwnctl
  ]
}

resource "docker_image" "pwnctl" {
  name         = "${aws_ecr_repository.main.repository_url}:latest"
  keep_locally = true

  build {
    context = "../src/core"
    dockerfile = "Dockerfile"
  }

  triggers = {
    dir_sha1 = sha1(join("", [for f in fileset(path.module, "../src/core/*") : filesha1(f)]))
  }
}