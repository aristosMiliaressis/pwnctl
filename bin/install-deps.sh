sudo apt-get install -y git curl wget jq docker nodejs

curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o awscliv2.zip
unzip awscliv2.zip
rm awscliv2.zip
./aws/install --update
rm -rf ./aws

curl -fsSL https://deb.nodesource.com/setup_18.x | bash - 

npm install -g aws-cdk

pip install awscurl

curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel 7.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64
ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

