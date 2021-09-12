FROM aaaguirrep/offensive-docker AS base

# install .NET runtime
RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        curl \
        ca-certificates \
        \
        # .NET dependencies
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu66 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
    && rm -rf /var/lib/apt/lists/*

RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel 5.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64 \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

RUN apt-get update && apt-get install -y apt-utils \
	 python3 \
	 python3-pip \
	 sqlite3 \
	 jq \
	 git \
	 masscan \
	 nmap \
	 gcc \
	 make \
	 golang \
     sudo

RUN pip3 install datasette

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY ["Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj", "Pwntainer.AssetArchiver/"]
RUN dotnet restore "Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj"
COPY . .
COPY Pwntainer.AssetArchiver/aliases.txt .
RUN dotnet build "/Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj" -c Release -o /app/build
RUN dotnet publish "/Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj" -r linux-x64 -p:PublishSingleFile=true -c Release -o /app/publish

FROM base AS release

ENV PATH $PATH:$GOPATH

WORKDIR /opt

RUN git clone https://github.com/vortexau/dnsvalidator.git \
    && cd dnsvalidator && python3 setup.py install

RUN git clone https://github.com/aristosMiliaressis/recon_scripts.git \
    && mv /opt/recon_scripts/* /usr/local/bin \
    && rm -r /opt/recon_scripts/

RUN mkdir /opt/pwntainer/
RUN printf ".databases\n.quit" | sqlite3 /opt/pwntainer/pwntainer.db

RUN ln -s /app/Pwntainer.AssetArchiver /usr/local/bin/aar

WORKDIR /app

COPY --from=build /app/publish .

#RUN cat aliases.txt >> /root/.bashrc 

WORKDIR /opt
RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
