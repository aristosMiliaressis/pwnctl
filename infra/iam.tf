

data "aws_iam_policy_document" "sqs_rw_policy" {
  statement {
    effect = "Allow"

    actions = [
      "sqs:ChangeMessageVisibility",
          "sqs:DeleteMessage",
          "sqs:GetQueueAttributes",
          "sqs:GetQueueUrl",
          "sqs:ReceiveMessage",
          "sqs:SendMessage"
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "sqs_rw_policy" {
  name        = "sqs_rw_policy"
  path        = "/"
  description = "IAM policy for sqs Read/Write access"
  policy      = data.aws_iam_policy_document.sqs_rw_policy.json
}


data "aws_iam_policy_document" "api_logging" {
  statement {
    effect = "Allow"

    actions = [
      "logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
    ]

    resources = ["arn:aws:logs:*:*:*"]
  }
}

resource "aws_iam_policy" "api_logging" {
  name        = "api_logging"
  path        = "/"
  description = "IAM policy for logging from a lambda"
  policy      = data.aws_iam_policy_document.api_logging.json
}