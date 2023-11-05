
resource "aws_ecr_repository" "exec" {
  name                 = "pwnctl-exec"

  tags = {
    Description = "Task executor image repository."
  }
}

resource "aws_ecr_lifecycle_policy" "exec" {
  repository = aws_ecr_repository.exec.name

  policy = <<EOF
{
  "rules": [
    {
      "rulePriority": 1,
      "description": "Keep last 10 images",
      "selection": {
        "tagStatus": "any",
        "countType": "imageCountMoreThan",
        "countNumber": 10
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
      "description": "Keep last 10 images",
      "selection": {
        "tagStatus": "any",
        "countType": "imageCountMoreThan",
        "countNumber": 10
      },
      "action": {
        "type": "expire"
      }
    }
  ]
}
EOF
}
