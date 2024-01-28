data "aws_ecr_repository" "exec_short" {
  name = "pwnctl-exec-short"
}

data "aws_ecr_repository" "exec_long" {
  name = "pwnctl-exec-long"
}

data "aws_ecr_repository" "proc" {
  name = "pwnctl-proc"
}

data "external" "commit_hash" {
  program = ["bash", "-c", "git rev-parse --short HEAD | jq --raw-input '. | { sha: (.) }'"]
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

locals {
  proc_img_src_path   = "${path.module}/../src/core"
  exec_short_img_src_path   = "${path.module}/../src/core"
  exec_long_img_src_path   = "${path.module}/../src/core"
  
  proc_img_src_sha256 = sha256(join("", [for f in fileset(".", "${local.proc_img_src_path}/**") : filebase64(f)]))
  exec_short_img_src_sha256 = sha256(join("", [for f in fileset(".", "${local.exec_short_img_src_path}/**") : filebase64(f)]))
  exec_long_img_src_sha256 = sha256(join("", [for f in fileset(".", "${local.exec_long_img_src_path}/**") : filebase64(f)]))

  proc_build_cmd = <<-EOT
    docker build -t ${data.aws_ecr_repository.proc.repository_url}:${data.external.commit_hash.result.sha} \
          --build-arg COMMIT_HASH=${data.external.commit_hash.result.sha} \
          -f ${local.proc_img_src_path}/pwnctl.proc/Dockerfile ${local.proc_img_src_path}
  EOT

  exec_short_build_cmd = <<-EOT
    docker build -t ${data.aws_ecr_repository.exec_short.repository_url}:${data.external.commit_hash.result.sha} \
          --build-arg COMMIT_HASH=${data.external.commit_hash.result.sha} --ssh default=$SSH_AUTH_SOCK \
          -f ${local.exec_short_img_src_path}/pwnctl.exec/shortlived/Dockerfile ${local.exec_short_img_src_path}
  EOT

  exec_long_build_cmd = <<-EOT
    docker build -t ${data.aws_ecr_repository.exec_long.repository_url}:${data.external.commit_hash.result.sha} \
          --build-arg COMMIT_HASH=${data.external.commit_hash.result.sha} --ssh default=$SSH_AUTH_SOCK \
          -f ${local.exec_long_img_src_path}/pwnctl.exec/longlived/Dockerfile ${local.exec_long_img_src_path}
  EOT
}

resource "null_resource" "build_push_proc_img" {
  triggers = {
    detect_docker_source_changes = local.proc_img_src_sha256
  }

  provisioner "local-exec" {
    command = local.proc_build_cmd
  }
}

resource "null_resource" "build_push_exec_short_img" {
  triggers = {
    detect_docker_source_changes = local.exec_short_img_src_sha256
  }

  provisioner "local-exec" {
    command = local.exec_short_build_cmd
  }
}

resource "null_resource" "build_push_exec_long_img" {
  triggers = {
    detect_docker_source_changes = local.exec_long_img_src_sha256
  }

  provisioner "local-exec" {
    command = local.exec_long_build_cmd
  }
}