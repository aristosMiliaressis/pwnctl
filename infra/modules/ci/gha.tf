
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

