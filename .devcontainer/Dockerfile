FROM ubuntu

ARG USERNAME=dev
ARG USER_UID=1000
ARG USER_GID=$USER_UID

RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    && apt-get update \
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME \
    && chmod 0440 /etc/sudoers.d/$USERNAME

USER $USERNAME

RUN sudo apt-get update \
    && sudo apt-get install -y amazon-ecr-credential-helper \
        curl \
        docker \
        dotnet-sdk-7.0 \
        gh \
        git \
        jq \
        nano \
        pip \
        unzip

RUN sh -c "$(curl --location https://taskfile.dev/install.sh)" -- -d

COPY --from=docker/buildx-bin /buildx /usr/libexec/docker/cli-plugins/docker-buildx
RUN docker buildx install

RUN curl -SL https://github.com/docker/compose/releases/download/v2.23.3/docker-compose-linux-x86_64 -o /usr/local/bin/docker-compose
RUN chmod +x /usr/local/bin/docker-compose

RUN curl -OJ https://releases.hashicorp.com/terraform/1.6.5/terraform_1.6.5_linux_amd64.zip
RUN unzip terraform_1.6.5_linux_amd64.zip \
    && rm terraform_1.6.5_linux_amd64.zip \
    && mv terraform /usr/local/bin/

RUN curl -OJ https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip
RUN unzip awscli-exe-linux-x86_64.zip \
    && rm awscli-exe-linux-x86_64.zip \
    && ./aws/install --update \
    && rm -rf ./aws

RUN pip install awslogs
