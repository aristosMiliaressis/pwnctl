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
     | bash /dev/stdin -Channel 6.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64 \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

RUN apt-get update && apt-get install -y apt-utils gcc sudo nano moreutils

RUN cd /tools/recon/massdns \
    && make \
    && ln -s /tools/recon/massdns/bin/massdns /usr/local/bin/ \
    && ln -s /tools/recon/massdns/scripts/subbrute.py /usr/local/bin/

WORKDIR /opt

RUN git clone https://github.com/vortexau/dnsvalidator.git \
    && cd dnsvalidator && python3 setup.py install

RUN go get -u github.com/tomnomnom/unfurl

RUN GO111MODULE=on go get -v github.com/projectdiscovery/shuffledns/cmd/shuffledns

RUN mkdir -p /opt/pwntainer/data/ \
 && mkdir -p /opt/pwntainer/wordlists/

RUN wget https://github.com/RustScan/RustScan/releases/download/2.0.1/rustscan_2.0.1_amd64.deb \
    && dpkg -i rustscan_2.0.1_amd64.deb && rm rustscan_2.0.1_amd64.deb

RUN go get -u github.com/glebarez/cero
RUN go install github.com/lc/gau/v2/cmd/gau@latest

RUN git clone https://github.com/dcsync/recontools.git /opt/pwntainer/recontools
RUN git clone https://github.com/danielmiessler/SecLists.git /opt/pwntainer/wordlists
RUN wget -O /opt/pwntainer/wordlists/commonspeak2.txt https://raw.githubusercontent.com/assetnote/commonspeak2-wordlists/master/subdomains/subdomains.txt

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY ["/src/pwnctl.core/pwnctl.core.csproj", "/src/pwnctl.core/"]
RUN dotnet restore "/src/pwnctl.core/pwnctl.core.csproj"
COPY ["/src/pwnctl.infra/pwnctl.infra.csproj", "/src/pwnctl.infra/"]
RUN dotnet restore "/src/pwnctl.infra/pwnctl.infra.csproj"
COPY ["/src/pwnctl.app/pwnctl.app.csproj", "/src/pwnctl.app/"]
RUN dotnet restore "/src/pwnctl.app/pwnctl.app.csproj"
COPY ["/src/pwnctl/pwnctl.csproj", "/src/pwnctl/"]
RUN dotnet restore "/src/pwnctl/pwnctl.csproj"

COPY . .

RUN dotnet build "/src/pwnctl/pwnctl.csproj" -c Release -o /app/build
RUN dotnet publish "/src/pwnctl/pwnctl.csproj" -r linux-x64 -c Release -o /app/publish # -p:PublishSingleFile=true

FROM base AS release

RUN wget -O /usr/local/bin/job-queue.sh https://raw.githubusercontent.com/aristosMiliaressis/job-queue.sh/master/job-queue.sh \
    && chmod +x /usr/local/bin/job-queue.sh

COPY --from=build /app/publish /app
RUN ln -s /app/pwnctl /usr/local/bin/pwnctl

RUN chmod -R +x /app/scripts \
    && mv /app/scripts/recon_scripts/* /usr/local/bin/ \
    && mv /app/scripts/* /usr/local/bin/
    
ENV INSTALL_PATH=/opt/pwntainer

RUN get_public_suffixes.sh


COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]