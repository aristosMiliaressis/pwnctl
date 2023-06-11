sudo apt-get install -y git gh curl wget jq docker

curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o awscliv2.zip
unzip awscliv2.zip
rm awscliv2.zip
./aws/install --update
rm -rf ./aws

curl -sSL https://dot.net/v1/dotnet-install.sh \
    | bash /dev/stdin -Channel 6.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64
ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
