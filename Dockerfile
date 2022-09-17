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
    && mkdir -p /opt/resources/wordlists \
    && mkdir /opt/resources/scripts

RUN wget https://github.com/RustScan/RustScan/releases/download/2.0.1/rustscan_2.0.1_amd64.deb \
    && dpkg -i rustscan_2.0.1_amd64.deb && rm rustscan_2.0.1_amd64.deb

RUN curl https://dl.google.com/go/go1.19.1.linux-amd64.tar.gz -o /tmp/go.tar.gz \
    && rm -rf /usr/local/go && tar -C /usr/local -xzf /tmp/go.tar.gz

RUN go install github.com/glebarez/cero@latest
RUN go install github.com/lc/gau/v2/cmd/gau@latest
RUN go install github.com/hakluke/hakrawler@latest
RUN go install github.com/tomnomnom/anew@latest
RUN go install github.com/d3mondev/puredns/v2@latest
RUN go install github.com/projectdiscovery/nuclei/v2/cmd/nuclei@latest
RUN go install github.com/aristosMiliaressis/urgo@latest
RUN go install github.com/projectdiscovery/dnsx/cmd/dnsx@latest
RUN git clone https://github.com/zmap/zdns.git && cd zdns && go build && cp zdns /usr/bin
RUN pip install useragent-picker-cli && uagen

RUN git clone https://github.com/ProjectAnte/dnsgen && cd dnsgen && pip3 install -r requirements.txt && python3 setup.py install
RUN git clone https://github.com/dcsync/recontools.git /opt/recontools
RUN git clone https://github.com/danielmiessler/SecLists.git /opt/resources/wordlists
RUN wget -O /opt/resources/wordlists/commonspeak2.txt https://raw.githubusercontent.com/assetnote/commonspeak2-wordlists/master/subdomains/subdomains.txt
RUN curl -L https://github.com/OWASP/Amass/releases/download/v3.19.3/amass_linux_amd64.zip -o /tmp/amass_linux_amd64.zip \
    && unzip -o /tmp/amass_linux_amd64.zip  -d /tools/recon/amass/amass_linux_amd64/ && rm /tmp/amass_linux_amd64.zip
RUN git clone https://github.com/darkoperator/dnsrecon.git \
    && cd dnsrecon && pip install -r requirements.txt && python3 setup.py install
RUN git clone https://github.com/initstring/cloud_enum.git && cd cloud_enum && pip install -r requirements.txt

RUN pip3 install arjun
RUN apt-get install -y ldnsutils

RUN git clone https://github.com/xnl-h4ck3r/xnLinkFinder.git \
    && cd xnLinkFinder \
    && python3 setup.py install

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY ["/src/pwnctl.core/pwnctl.core.csproj", "/src/pwnctl.core/"]
RUN dotnet restore "/src/pwnctl.core/pwnctl.core.csproj"
COPY ["/src/pwnctl.infra/pwnctl.infra.csproj", "/src/pwnctl.infra/"]
RUN dotnet restore "/src/pwnctl.infra/pwnctl.infra.csproj"
COPY ["/src/pwnctl.app/pwnctl.app.csproj", "/src/pwnctl.app/"]
RUN dotnet restore "/src/pwnctl.app/pwnctl.app.csproj"
COPY ["/src/pwnctl.cli/pwnctl.cli.csproj", "/src/pwnctl.cli/"]
RUN dotnet restore "/src/pwnctl.cli/pwnctl.cli.csproj"

COPY src/ src/

RUN dotnet build "/src/pwnctl.cli/pwnctl.cli.csproj" -c Release -o /app/build
RUN dotnet publish "/src/pwnctl.cli/pwnctl.cli.csproj" -r linux-x64 -c Release -o /app/publish # -p:PublishSingleFile=true

FROM base AS release

COPY --from=build /app/publish /app
RUN ln -s /app/pwnctl.cli /usr/local/bin/pwnctl

RUN wget -O /usr/local/bin/job-queue.sh https://raw.githubusercontent.com/aristosMiliaressis/job-queue.sh/master/job-queue.sh \
    && chmod +x /usr/local/bin/job-queue.sh

COPY resources/scripts/* /opt/resources/scripts/
COPY resources/wordlists/ /opt/resources/wordlists/
RUN chmod -R +x /opt/resources/scripts \
    && mv /opt/resources/scripts/* /usr/local/bin/
    
ENV PWNCTL_INSTALL_PATH "/opt/pwntainer"

COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
