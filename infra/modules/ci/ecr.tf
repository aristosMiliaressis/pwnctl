
resource "aws_ecr_repository" "exec_long" {
  name                 = "pwnctl-exec-long"
  force_delete         = true

  tags = {
    Description = "Task long lived executor image repository."
  }
}

resource "aws_ecr_lifecycle_policy" "exec_long" {
  repository = aws_ecr_repository.exec_long.name

  policy = <<EOF
{
  "rules": [
    {
      "rulePriority": 1,
      "description": "Keep last 5 images",
      "selection": {
        "tagStatus": "any",
        "countType": "imageCountMoreThan",
        "countNumber": 5
      },
      "action": {
        "type": "expire"
      }
    }
  ]
}
EOF
}

resource "aws_ecr_repository" "exec_short" {
  name                 = "pwnctl-exec-short"
  force_delete         = true

  tags = {
    Description = "Task short lived executor image repository."
  }
}

resource "aws_ecr_lifecycle_policy" "exec_short" {
  repository = aws_ecr_repository.exec_short.name

  policy = <<EOF
{
  "rules": [
    {
      "rulePriority": 1,
      "description": "Keep last 5 images",
      "selection": {
        "tagStatus": "any",
        "countType": "imageCountMoreThan",
        "countNumber": 5
      },
      "action": {
        "type": "expire"
      }
    }
  ]
}
EOF
}

resource "aws_ecr_repository" "proc" {
  name                 = "pwnctl-proc"
  force_delete         = true

  tags = {
    Description = "output processor image repository."
  }
}

resource "aws_ecr_lifecycle_policy" "proc" {
  repository = aws_ecr_repository.proc.name

  policy = <<EOF
{
  "rules": [
    {
      "rulePriority": 1,
      "description": "Keep last 5 images",
      "selection": {
        "tagStatus": "any",
        "countType": "imageCountMoreThan",
        "countNumber": 5
      },
      "action": {
        "type": "expire"
      }
    }
  ]
}
EOF
}
