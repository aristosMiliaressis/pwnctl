FROM aaaguirrep/offensive-docker AS base

# install .NET runtime
RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        ca-certificates \
        # .NET dependencies
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu66 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
    && rm -rf /var/lib/apt/lists/*

RUN curl -sSL https://dot.net/v1/dotnet-install.sh \
     | bash /dev/stdin -Channel 5.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64 \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

RUN apt-get update && apt-get install -y apt-utils gcc sudo nano

RUN pip3 install datasette

WORKDIR /opt

RUN git clone https://github.com/vortexau/dnsvalidator.git \
    && cd dnsvalidator && python3 setup.py install

RUN go get -u github.com/tomnomnom/unfurl

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY ["Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj", "Pwntainer.AssetArchiver/"]
RUN dotnet restore "Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj"
COPY . .
RUN dotnet build "/Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj" -c Release -o /app/build
RUN dotnet publish "/Pwntainer.AssetArchiver/Pwntainer.AssetArchiver.csproj" -r linux-x64 -c Release -o /app/publish # -p:PublishSingleFile=true

FROM base AS release

COPY --from=build /app/publish /app

RUN ln -s /app/Pwntainer.AssetArchiver /usr/local/bin/aar

WORKDIR /opt

#RUN git clone https://github.com/aristosMiliaressis/recon_scripts.git \
    #&& mv /opt/recon_scripts/* /usr/local/bin \
    #&& rm -r /opt/recon_scripts/

RUN mkdir -p /opt/pwntainer/data/
RUN printf ".databases\n.quit" | sqlite3 /opt/pwntainer/pwntainer.db

RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
