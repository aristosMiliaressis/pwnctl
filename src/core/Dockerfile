FROM ubuntu AS base

# Installing system,build,network utils
RUN apt-get update \
    && apt-get install -y apt-utils \
    cron \
    curl \
    dnsutils \
    gcc \
    git \
    groff \
    jq \
    libpcap-dev \
    make \
    moreutils \
    nano \
    openssl \
    sudo \
    unzip \
    wget

# Setting up .NET runtime
RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
    ca-certificates \
    # .NET dependencies
    libc6 \
    libgcc1 \
    libgssapi-krb5-2 \
    libstdc++6 \
    zlib1g \
    && rm -rf /var/lib/apt/lists/*

RUN curl -sSL https://dot.net/v1/dotnet-install.sh \
    | bash /dev/stdin -Channel 6.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64 \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Setting up python
RUN apt-get update \
    && apt-get install -y python3 \
    python3-pip \
    python3-setuptools

RUN python3 -m pip install --upgrade pip

# Setting up Ruby
RUN apt-get install -y ruby ruby-dev \
    && gem install bundler

# Setting up java
RUN apt-get install -y openjdk-8-jdk

# Setting up Go lang
COPY --from=golang:1.19-buster /usr/local/go/ /usr/local/go/
ENV GOROOT "/usr/local/go"
ENV GOPATH "/root/go"
ENV PATH "$PATH:$GOPATH/bin:$GOROOT/bin"

# Setting up the aws cli
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o awscliv2.zip \
    && unzip awscliv2.zip \
    && rm awscliv2.zip \
    && ./aws/install --update \
    && rm -rf ./aws

# Setting up xvfb & chromium for headless browsing
RUN apt-get update \
    && apt-get install -y libgbm1 \
        libxkbcommon0 \
        xvfb

# install chromium
WORKDIR /opt/chromium
RUN apt-get install -yq gconf-service libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 \
    libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 \
    libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 \
    libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 \
    ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils wget

RUN git clone https://github.com/scheib/chromium-latest-linux
WORKDIR /opt/chromium/chromium-latest-linux
RUN ./update.sh && ln -s /opt/chromium/chromium-latest-linux/latest/chrome /usr/bin/chromium

# Installing recon tools
RUN mkdir -p /opt/wordlists/dns/ \
 && mkdir /opt/scripts \
 && mkdir /opt/tools

RUN apt-get install -y nmap whois ldnsutils

RUN go install github.com/tomnomnom/unfurl@latest
RUN go install github.com/tomnomnom/anew@latest
RUN go install github.com/tomnomnom/waybackurls@latest
RUN go install github.com/projectdiscovery/tlsx/cmd/tlsx@latest
RUN go install github.com/projectdiscovery/dnsx/cmd/dnsx@latest
RUN go install github.com/projectdiscovery/httpx/cmd/httpx@latest
RUN go install github.com/projectdiscovery/notify/cmd/notify@latest
RUN go install github.com/projectdiscovery/katana/cmd/katana@latest
#RUN go install github.com/projectdiscovery/naabu/v2/cmd/naabu@latest
RUN go install github.com/lc/gau/v2/cmd/gau@latest
RUN go install github.com/hakluke/hakrawler@latest
RUN go install github.com/d3mondev/puredns/v2@latest
RUN go install github.com/aristosMiliaressis/urgo@latest
RUN go install github.com/ffuf/ffuf@latest

RUN pip install useragent-picker-cli && uagen
RUN mkdir -p /etc/amass/scripts \
    && git clone https://github.com/PatrikFehrenbach/amass-tools.git /etc/amass/scripts
RUN wget https://github.com/RustScan/RustScan/releases/download/2.0.1/rustscan_2.0.1_amd64.deb \
    && dpkg -i rustscan_2.0.1_amd64.deb && rm rustscan_2.0.1_amd64.deb
RUN curl -o /opt/wordlists/common.txt https://raw.githubusercontent.com/danielmiessler/SecLists/master/Discovery/Web-Content/common.txt
RUN curl -o /opt/wordlists/commonspeak2.txt https://raw.githubusercontent.com/assetnote/commonspeak2-wordlists/master/subdomains/subdomains.txt
RUN curl -o /opt/wordlists/subdomains-top-20000.txt https://raw.githubusercontent.com/theMiddleBlue/DNSenum/master/wordlist/subdomains-top1mil-20000.txt
WORKDIR /opt/tools

RUN git clone https://github.com/zmap/zdns.git && cd zdns && go build && cp zdns /usr/bin
RUN git clone https://github.com/blechschmidt/massdns.git \
    && cd massdns && make \
    && ln -s /opt/tools/massdns/bin/massdns /usr/local/bin/ \
    && ln -s /opt/tools/massdns/scripts/subbrute.py /usr/local/bin/
RUN git clone https://github.com/vortexau/dnsvalidator.git \
    && cd dnsvalidator && python3 setup.py install
RUN git clone https://github.com/s0md3v/Corsy.git \
    && pip3 install -r Corsy/requirements.txt \
    && chmod +x Corsy/corsy.py \
    && ln -s /opt/tools/Corsy/corsy.py /usr/local/bin
RUN git clone https://github.com/cramppet/regulator && cd regulator && pip3 install -r requirements.txt
RUN pip3 install py-altdns==1.0.2
RUN git clone https://github.com/dcsync/recontools.git
RUN curl -L https://github.com/OWASP/Amass/releases/download/v3.19.3/amass_linux_amd64.zip -o /tmp/amass_linux_amd64.zip \
    && unzip -o /tmp/amass_linux_amd64.zip -d /opt/tools/amass/ \
    && rm /tmp/amass_linux_amd64.zip \
    && ln -s /opt/tools/amass/amass_linux_amd64/amass /usr/local/bin
RUN git clone https://github.com/darkoperator/dnsrecon.git \
    && cd dnsrecon && pip install -r requirements.txt && python3 setup.py install
RUN git clone https://github.com/initstring/cloud_enum.git \
    && cd cloud_enum \
    && pip install -r requirements.txt
RUN git clone https://github.com/xnl-h4ck3r/xnLinkFinder.git \
    && cd xnLinkFinder \
    && python3 setup.py install
RUN git clone https://github.com/EnableSecurity/wafw00f.git \
    && cd wafw00f && python3 setup.py install
RUN go install github.com/rverton/webanalyze/cmd/webanalyze@latest
RUN go install github.com/sw33tLie/sns@latest
RUN go install github.com/projectdiscovery/nuclei/v2/cmd/nuclei@latest
RUN go install github.com/gwen001/github-subdomains@latest
RUN pip3 install s3scanner
RUN pip3 install emailfinder

RUN curl https://github.com/mgdm/htmlq/releases/download/v0.4.0/htmlq-x86_64-linux.tar.gz -L -o htmlq-x86_64-linux.tar.gz \
    && tar xzf htmlq-x86_64-linux.tar.gz \
    && rm htmlq-x86_64-linux.tar.gz && mv htmlq /usr/local/bin

RUN curl https://api.github.com/repos/michenriksen/aquatone/releases/latest 2>/dev/null \
    | grep browser_download_url \
    | awk '{print $2}' \
    | tr -d '"' \
    | grep 'aquatone_linux_amd64' \
    | wget -qi - \
    && unzip aquatone_linux_amd64* \
    && mv aquatone /usr/local/bin
RUN git clone https://github.com/xnl-h4ck3r/waymore.git \
    && cd waymore \
    && python3 setup.py install
RUN go install github.com/detectify/page-fetch@latest

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# building worker service & cli
COPY ["/pwnctl.kernel/pwnctl.kernel.csproj", "/src/pwnctl.kernel/"]
RUN dotnet restore "/src/pwnctl.kernel/pwnctl.kernel.csproj"
COPY ["/pwnctl.domain/pwnctl.domain.csproj", "/src/pwnctl.domain/"]
RUN dotnet restore "/src/pwnctl.domain/pwnctl.domain.csproj"
COPY ["/pwnctl.app/pwnctl.app.csproj", "/src/pwnctl.app/"]
RUN dotnet restore "/src/pwnctl.app/pwnctl.app.csproj"
COPY ["/pwnctl.infra/pwnctl.infra.csproj", "/src/pwnctl.infra/"]
RUN dotnet restore "/src/pwnctl.infra/pwnctl.infra.csproj"
COPY ["/pwnctl.svc/pwnctl.svc.csproj", "/src/pwnctl.svc/"]
RUN dotnet restore "/src/pwnctl.svc/pwnctl.svc.csproj"

COPY . src/

RUN dotnet publish "/src/pwnctl.svc/pwnctl.svc.csproj" -c Release -o /publish/svc

FROM base AS release

COPY --from=build /publish/svc /opt/pwnctl-svc
COPY pwnctl.svc/wordlists/ /opt/wordlists/
COPY pwnctl.svc/nuclei_templates/ /opt/nuclei_templates/
COPY pwnctl.svc/scripts/* /opt/scripts/
RUN chmod -R +x /opt/scripts \
    && mv /opt/scripts/* /usr/local/bin/

COPY pwnctl.svc/entrypoint.sh /opt/pwnctl-svc/entrypoint.sh
RUN chmod +x /opt/pwnctl-svc/entrypoint.sh
ENTRYPOINT ["/opt/pwnctl-svc/entrypoint.sh"]
