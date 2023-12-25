FROM ubuntu

RUN apt-get update \
    && apt-get install -y amazon-ecr-credential-helper \
        curl \
        docker \
        dotnet-sdk-7.0 \
        gh \
        git \
        jq \
        nano \
        unzip

RUN sh -c "$(curl --location https://taskfile.dev/install.sh)" -- -d

RUN curl -OJ https://releases.hashicorp.com/terraform/1.6.5/terraform_1.6.5_linux_amd64.zip
RUN unzip terraform_1.6.5_linux_amd64.zip \
    && rm terraform_1.6.5_linux_amd64.zip \
    && mv terraform /usr/local/bin/

RUN curl -OJ https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip
RUN unzip awscli-exe-linux-x86_64.zip \
    && rm awscli-exe-linux-x86_64.zip \
    && ./aws/install --update \
    && rm -rf ./aws

RUN curl -OJ https://amazon-ecs-cli.s3.amazonaws.com/ecs-cli-linux-amd64-latest \
    && chmod +x ecs-cli-linux-amd64-latest \
    && mv ecs-cli-linux-amd64-latest /usr/local/bin/ecs-cli

COPY --from=docker/buildx-bin /buildx /usr/libexec/docker/cli-plugins/docker-buildx
RUN docker buildx install

RUN curl -SL https://github.com/docker/compose/releases/download/v2.23.3/docker-compose-linux-x86_64 -o /usr/local/bin/docker-compose
RUN chmod +x /usr/local/bin/docker-compose
