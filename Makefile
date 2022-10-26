SHELL := /bin/bash
dotnet := dotnet
region := $(shell aws configure get region)
accountId := $(shell aws sts get-caller-identity --query "Account" --output text)
ecrRepo := $(shell echo) # aws ecr describe-repositories | jq .repositories[]
connectionString := $(shell echo) # get from ssm


simple-setup: install-deps build-all install-cli-src build-svc-image push-svc-image bootstrap-aws deploy-app deploy-files

test:
	${dotnet} test

build-api:
	${dotnet} build src/pwnctl/pwnctl.api/pwnctl.api.csproj -c Release

build-pwnctl-cli:
	${dotnet} publish src/pwnctl/pwnctl.cli/pwnctl.cli.csproj -c Release

build-pwnwrk-cli:
	${dotnet} publish src/pwnwrk/pwnwrk.cli/pwnwrk.cli.csproj -c Release

build-svc:
	${dotnet} publish src/pwnwrk/pwnwrk.svc/pwnwrk.svc.csproj -c Release

build-svc-image:
	docker build src/ -t ${ecrRepo}/pwnwrk:latest

push-svc-image:
	aws ecr-public get-login-password --region ${region} | docker login --username AWS --password-stdin ${ecrRepo}
	docker push ${ecrRepo}/pwnwrk:latest

build-all: build-pwnctl-cli build-api build-svc-image

install-cli-src: build-pwnctl-cli
	chmod +x src/pwnctl/pwnctl.cli/bin/Release/net6.0/linux-x64/pwnctl
	mv src/pwnctl/pwnctl.cli/bin/Release/net6.0/linux-x64/pwnctl /usr/local/bin

install-deps:
	sudo apt-get install -y git curl wget jq docker nodejs
	curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o awscliv2.zip
	unzip awscliv2.zip
	rm awscliv2.zip
	./aws/install --update
	rm -rf ./aws
	curl -fsSL https://deb.nodesource.com/setup_18.x | bash - 
	npm install -g aws-cdk
	curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel 6.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64
	ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

bootstrap-aws:
	cdk bootstrap aws://${accountId}/${region}

deploy-app: clean-cdk build-api
	cdk deploy PwnctlCdkStack -o aws/pwnctl.cdk/cdk.out --parameters connectionString="${connectionString}"

deploy-files:
	./aws/deploy.sh

clean-cdk:
	rm -rf ./aws/pwnctl.cdk/cdk.out

destroy:
	cdk destroy -o aws/pwnctl.cdk/cdk.out