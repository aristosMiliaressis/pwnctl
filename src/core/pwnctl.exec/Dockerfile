FROM ubuntu AS tool-build

RUN apt-get update \
    && apt-get install -y git make unzip musl-dev gcc curl

# Setting up Go lang
COPY --from=golang:1.20 /usr/local/go/ /usr/local/go/
ENV GOROOT "/usr/local/go"
ENV GOPATH "/root/go"
ENV PATH "$PATH:$GOPATH/bin:$GOROOT/bin"

RUN go install github.com/tomnomnom/anew@latest
RUN curl https://raw.githubusercontent.com/aristosMiliaressis/wordlist_builder/master/wordlist_builder.sh | bash -s -- /opt/wordlists/
RUN go install github.com/tomnomnom/unfurl@latest
RUN go install github.com/projectdiscovery/tlsx/cmd/tlsx@latest
RUN go install github.com/projectdiscovery/dnsx/cmd/dnsx@latest
RUN go install github.com/projectdiscovery/httpx/cmd/httpx@latest
RUN go install github.com/projectdiscovery/katana/cmd/katana@latest
RUN go install github.com/projectdiscovery/nuclei/v2/cmd/nuclei@latest
RUN go install github.com/projectdiscovery/cdncheck/cmd/cdncheck@latest
RUN go install github.com/projectdiscovery/asnmap/cmd/asnmap@latest
RUN go install github.com/projectdiscovery/mapcidr/cmd/mapcidr@latest
#RUN go install github.com/projectdiscovery/naabu/v2/cmd/naabu@latest
#RUN go install github.com/ffuf/ffuf@latest
RUN go install github.com/lc/gau/v2/cmd/gau@latest
RUN go install github.com/hakluke/hakrawler@latest
RUN go install github.com/d3mondev/puredns/v2@latest
RUN go install github.com/aristosMiliaressis/urgo@latest
RUN go install github.com/rverton/webanalyze/cmd/webanalyze@latest
RUN go install github.com/sw33tLie/sns@latest
RUN go install github.com/sensepost/gowitness@latest
RUN go install github.com/zmap/zdns@latest
RUN go install github.com/owasp-amass/amass/v3/...@master
RUN go install github.com/tomnomnom/assetfinder@latest
RUN go install github.com/gwen001/github-subdomains@latest
RUN go install github.com/projectdiscovery/subfinder/v2/cmd/subfinder@latest
RUN go install github.com/Josue87/gotator@latest
RUN go install github.com/projectdiscovery/notify/cmd/notify@latest
RUN go install github.com/aristosMiliaressis/mdwfuzzer@latest

WORKDIR /tmp
RUN git clone https://github.com/aristosMiliaressis/ffuf.git && go build -C ffuf && mv ffuf/ffuf /root/go/bin
RUN git clone https://github.com/blechschmidt/massdns.git \
    && cd massdns && make && cp bin/massdns  /root/go/bin/

# Installing private tools
RUN mkdir -p /opt/wordlists/dns/
RUN mkdir -p -m 0700 ~/.ssh && ssh-keyscan github.com >> ~/.ssh/known_hosts
RUN --mount=type=ssh \
    git clone git@github.com:aristosMiliaressis/vhost-brute.git && cd vhost-brute && go build cmd/vhost-brute/vhost-brute.go && cp vhost-brute /root/go/bin
RUN --mount=type=ssh \
    git clone git@github.com:aristosMiliaressis/cache-prober.git && cd cache-prober && make build && cp main /root/go/bin/cache-prober
RUN --mount=type=ssh \
    git clone -b pwnctl git@github.com:aristosMiliaressis/sub-enum.git \
    && cp sub-enum/wordlists/* /opt/wordlists/dns/ && cp sub-enum/sub-enum.sh /root/go/bin/

FROM golang:1.20.2-alpine as nuclei-templates

RUN go install github.com/xm1k3/cent@latest

RUN go install github.com/projectdiscovery/nuclei/v2/cmd/nuclei@latest
RUN mkdir -p /opt/nuclei_templates/{community,official}
RUN nuclei -ud /opt/nuclei_templates/official
RUN cent init
RUN cent -t=20 -p=/opt/nuclei_templates/community


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS worker-build

# building exec service
COPY ["/pwnctl.kernel/pwnctl.kernel.csproj", "/src/pwnctl.kernel/"]
RUN dotnet restore "/src/pwnctl.kernel/pwnctl.kernel.csproj"
COPY ["/pwnctl.domain/pwnctl.domain.csproj", "/src/pwnctl.domain/"]
RUN dotnet restore "/src/pwnctl.domain/pwnctl.domain.csproj"
COPY ["/pwnctl.app/pwnctl.app.csproj", "/src/pwnctl.app/"]
RUN dotnet restore "/src/pwnctl.app/pwnctl.app.csproj"
COPY ["/pwnctl.infra/pwnctl.infra.csproj", "/src/pwnctl.infra/"]
RUN dotnet restore "/src/pwnctl.infra/pwnctl.infra.csproj"
COPY ["/pwnctl.exec/pwnctl.exec.csproj", "/src/pwnctl.exec/"]
RUN dotnet restore "/src/pwnctl.exec/pwnctl.exec.csproj"

COPY . src/

RUN dotnet publish "/src/pwnctl.exec/pwnctl.exec.csproj" -c Release -o /publish/exec


FROM ubuntu AS release

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
    | bash /dev/stdin -Channel 7.0 -Runtime aspnetcore -InstallDir /usr/share/dotnet --architecture x64 \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Setting up java
RUN apt-get update && apt-get install -y openjdk-8-jdk

# Setting up python
RUN apt-get update \
    && apt-get install -y python3 \
    python3-pip \
    python3-setuptools

# Setting up xvfb for headless browsing
WORKDIR /opt/chromium

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgbm1 libxkbcommon0 \
    xvfb gconf-service libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 \
    libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 \
    libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 \
    libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 \
    ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils curl wget nmap git

# Installing tools
RUN mkdir /opt/tools
WORKDIR /opt/tools

RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y jq whois dnsutils ldnsutils less

RUN python3 -m pip install --upgrade pip
RUN cp /usr/bin/python3 /usr/bin/python

RUN pip3 install useragent-picker-cli && uagen
RUN pip3 install s3scanner
RUN pip3 install emailfinder
RUN python3 -m pip install -e git+https://github.com/EnableSecurity/wafw00f#egg=wafw00f
RUN python3 -m pip install -e git+https://github.com/initstring/cloud_enum#egg=cloud_enum
RUN python3 -m pip install -e git+https://github.com/xnl-h4ck3r/urless#egg=urless
RUN python3 -m pip install -e git+https://github.com/xnl-h4ck3r/waymore#egg=waymore \
    && ln /opt/tools/src/waymore/waymore.py /usr/local/bin/waymore && chmod +x /usr/local/bin/waymore
RUN python3 -m pip install -e git+https://github.com/brandonscholet/python-Wappalyzer.git#egg=python-Wappalyzer
RUN python3 -m pip install -e git+https://github.com/brandonscholet/wappybird.git#egg=wappybird \
    && pip3 install click validators \
    && ln /opt/tools/src/wappybird/wappybird/main.py /usr/local/bin/wappy && chmod +x /usr/local/bin/wappy
RUN git clone -b 1.1.4 https://github.com/darkoperator/dnsrecon.git \
    && cd dnsrecon && pip install -r requirements.txt && python3 setup.py install
RUN git clone https://github.com/cramppet/regulator \
    && pip3 install -r regulator/requirements.txt
RUN git clone https://github.com/s0md3v/Corsy.git \
    && pip3 install -r Corsy/requirements.txt \
    && chmod +x Corsy/corsy.py \
    && ln -s /opt/tools/Corsy/corsy.py /usr/local/bin
RUN curl -LOJ https://github.com/RustScan/RustScan/releases/download/2.0.1/rustscan_2.0.1_amd64.deb \
    && dpkg -i rustscan_2.0.1_amd64.deb && rm rustscan_2.0.1_amd64.deb
RUN curl https://github.com/mgdm/htmlq/releases/download/v0.4.0/htmlq-x86_64-linux.tar.gz -L -o htmlq-x86_64-linux.tar.gz \
    && tar xzf htmlq-x86_64-linux.tar.gz \
    && rm htmlq-x86_64-linux.tar.gz && mv htmlq /usr/local/bin
RUN mkdir -p /opt/toos/ShortNameScanner && cd /opt/toos/ShortNameScanner \
    && curl -OJ https://raw.githubusercontent.com/irsdl/IIS-ShortName-Scanner/master/release/iis_shortname_scanner.jar \
    && curl -OJ https://raw.githubusercontent.com/irsdl/IIS-ShortName-Scanner/master/release/config.xml
RUN git clone https://github.com/punk-security/dnsReaper.git && cd dnsReaper \
    && pip3 install -r requirements.txt

COPY --from=tool-build /root/go/bin/ /usr/local/bin/
COPY --from=tool-build /opt/wordlists/ /opt/wordlists/
COPY --from=tool-build /opt/wordlists/ /opt/wordlists/
COPY --from=nuclei-templates /opt/nuclei_templates/ /opt/nuclei_templates/
COPY --from=worker-build /publish/exec /opt/pwnctl-exec
COPY pwnctl.exec/wordlists/ /opt/wordlists/
COPY pwnctl.exec/nuclei_templates/ /opt/nuclei_templates/
COPY pwnctl.exec/scripts/* /opt/scripts/
COPY pwnctl.exec/entrypoint.sh /opt/pwnctl-exec/entrypoint.sh

RUN chmod -R +x /opt/scripts \
    && mv /opt/scripts/* /usr/local/bin/ \
    && mkdir -p ~/.config/ffuf/autocalibration \
    && mv /opt/wordlists/ffuf_ac_strategies/* ~/.config/ffuf/autocalibration

RUN chmod +x /opt/pwnctl-exec/entrypoint.sh
ENTRYPOINT ["/opt/pwnctl-exec/entrypoint.sh"]