FROM ubuntu

RUN apt-get update \
    && apt-get install -y curl \
        docker \
        dotnet-sdk-7.0 \
        gh \
        git \
        jq \
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