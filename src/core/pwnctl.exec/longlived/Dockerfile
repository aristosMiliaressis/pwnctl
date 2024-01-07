FROM ubuntu AS tool-build

RUN apt-get update \
    && apt-get install -y git make unzip musl-dev gcc curl libpcap-dev

# Setting up Go lang
COPY --from=golang:1.20 /usr/local/go/ /usr/local/go/
ENV GOROOT "/usr/local/go"
ENV GOPATH "/root/go"
ENV PATH "$PATH:$GOPATH/bin:$GOROOT/bin"

RUN go install github.com/tomnomnom/anew@latest
RUN go install github.com/tomnomnom/unfurl@latest
RUN go install github.com/projectdiscovery/katana/cmd/katana@latest
RUN go install github.com/projectdiscovery/nuclei/v2/cmd/nuclei@latest
RUN go install github.com/projectdiscovery/mapcidr/cmd/mapcidr@latest
RUN go install github.com/lc/gau/v2/cmd/gau@latest
RUN go install github.com/hakluke/hakrawler@latest
RUN go install github.com/d3mondev/puredns/v2@latest
RUN go install github.com/aristosMiliaressis/skiphost@latest
RUN go install github.com/zmap/zdns@latest
RUN go install github.com/owasp-amass/amass/v3/...@master
RUN go install github.com/tomnomnom/assetfinder@latest
RUN go install github.com/gwen001/github-subdomains@latest
RUN go install github.com/projectdiscovery/subfinder/v2/cmd/subfinder@latest
RUN go install github.com/Josue87/gotator@latest
RUN go install github.com/projectdiscovery/notify/cmd/notify@latest
RUN go install github.com/ffuf/ffuf@latest
RUN go install github.com/hakluke/hakip2host@latest
RUN go install github.com/projectdiscovery/naabu/v2/cmd/naabu@latest
RUN go install github.com/aristosMiliaressis/ffufClean@latest

WORKDIR /tmp
RUN git clone https://github.com/blechschmidt/massdns.git \
    && cd massdns && make && cp bin/massdns $GOPATH/bin/

RUN mkdir -p /opt/wordlists/dns/
RUN curl https://wordlists-cdn.assetnote.io/data/manual/best-dns-wordlist.txt -o /opt/wordlists/dns/subdomains-3m.txt

# Installing private tools
RUN mkdir -p -m 0700 ~/.ssh && ssh-keyscan github.com >> ~/.ssh/known_hosts
RUN --mount=type=ssh \
    git clone git@github.com:aristosMiliaressis/vhost-brute.git && make -C vhost-brute install
RUN --mount=type=ssh \
    git clone git@github.com:aristosMiliaressis/cache-prober.git && make -C cache-prober install
RUN --mount=type=ssh \
    git clone git@github.com:aristosMiliaressis/cors-scanner.git && make -C cors-scanner install
RUN --mount=type=ssh \
    git clone git@github.com:aristosMiliaressis/mdwfuzzer.git && make -C mdwfuzzer install
RUN --mount=type=ssh \
    git clone -b pwnctl git@github.com:aristosMiliaressis/sub-enum.git \
    && cp sub-enum/wordlists/* /opt/wordlists/dns/ && cp sub-enum/sub-enum.sh $GOPATH/bin/ \
    && cp sub-enum/sub-wildcard-finder.sh $GOPATH/bin/ && chmod +x $GOPATH/bin/sub-wildcard-finder.sh

FROM golang:1.20.2-alpine as nuclei-templates

RUN go install github.com/xm1k3/cent@latest

RUN go install github.com/projectdiscovery/nuclei/v2/cmd/nuclei@latest
RUN mkdir -p /opt/nuclei_templates/{community,official}
RUN nuclei -ud /opt/nuclei_templates/official
RUN cent init
RUN cent -t=20 -p=/opt/nuclei_templates/community


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS worker-build

# building exec service
WORKDIR /src
COPY ["/pwnctl.kernel/pwnctl.kernel.csproj", "/src/pwnctl.kernel/"]
RUN dotnet restore "/src/pwnctl.kernel/pwnctl.kernel.csproj"
COPY ["/pwnctl.domain/pwnctl.domain.csproj", "/src/pwnctl.domain/"]
RUN dotnet restore "/src/pwnctl.domain/pwnctl.domain.csproj"
COPY ["/pwnctl.app/pwnctl.app.csproj", "/src/pwnctl.app/"]
RUN dotnet restore "/src/pwnctl.app/pwnctl.app.csproj"
COPY ["/pwnctl.infra/pwnctl.infra.csproj", "/src/pwnctl.infra/"]
RUN dotnet restore "/src/pwnctl.infra/pwnctl.infra.csproj"
COPY ["/pwnctl.exec/longlived/pwnctl.exec.longlived.csproj", "/src/pwnctl.exec/longlived/"]
RUN dotnet restore "/src/pwnctl.exec/longlived/pwnctl.exec.longlived.csproj"

COPY . /src

RUN dotnet publish "/src/pwnctl.exec/longlived/pwnctl.exec.longlived.csproj" -c Release -o /src/publish

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
RUN apt-get update && apt-get install -y openjdk-17-jre

# Setting up python
RUN apt-get update \
    && apt-get install -y python3 \
    python3-pip \
    python3-setuptools
RUN cp /usr/bin/python3 /usr/bin/python

# Setting up xvfb for headless browsing
WORKDIR /opt/chromium

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgbm1 libxkbcommon0 libpcap-dev \
    xvfb gconf-service libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 \
    libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 \
    libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 \
    libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 \
    ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils curl git

# Installing tools
RUN mkdir /opt/tools
WORKDIR /opt/tools

RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y jq whois dnsutils less parallel wget nmap

RUN python3 -m pip install useragent-picker-cli && uagen
RUN python3 -m pip install -e git+https://github.com/initstring/cloud_enum#egg=cloud_enum
RUN python3 -m pip install -e git+https://github.com/xnl-h4ck3r/urless#egg=urless
RUN python3 -m pip install -e git+https://github.com/xnl-h4ck3r/waymore#egg=waymore \
    && ln /opt/tools/src/waymore/waymore.py \
    && chmod +x /opt/tools/src/waymore/waymore.py
RUN git clone https://github.com/cramppet/regulator \
    && python3 -m pip install -r regulator/requirements.txt
RUN git clone https://github.com/defparam/smuggler.git
RUN mkdir -p /opt/toos/ShortNameScanner && cd /opt/toos/ShortNameScanner \
    && curl -OJ https://raw.githubusercontent.com/irsdl/IIS-ShortName-Scanner/master/release/iis_shortname_scanner.jar \
    && curl -OJ https://raw.githubusercontent.com/irsdl/IIS-ShortName-Scanner/master/release/config.xml

COPY --from=tool-build /root/go/bin/ /usr/local/bin/
RUN curl https://raw.githubusercontent.com/aristosMiliaressis/wordlist_builder/master/wordlist_builder.sh | bash -s -- /opt/wordlists/

COPY --from=tool-build /opt/wordlists/ /opt/wordlists/
COPY --from=nuclei-templates /opt/nuclei_templates/ /opt/nuclei_templates/
COPY --from=worker-build /src/publish /opt/pwnctl-exec
COPY pwnctl.exec/wordlists/ /opt/wordlists/
COPY pwnctl.exec/nuclei_templates/ /opt/nuclei_templates/
COPY pwnctl.exec/scripts/* /opt/scripts/
COPY pwnctl.exec/longlived/entrypoint.sh /opt/pwnctl-exec/entrypoint.sh
RUN cat /opt/wordlists/dns/perms-large.txt | anew -q /opt/tools/src/cloud-enum/enum_tools/fuzz.txt

RUN chmod -R +x /opt/scripts \
    && mv /opt/scripts/* /usr/local/bin/ \
    && mkdir -p ~/.config/ffuf/autocalibration \
    && mv /opt/wordlists/ffuf_ac_strategies/* ~/.config/ffuf/autocalibration

ARG COMMIT_HASH
RUN echo "export PWNCTL_COMMIT_HASH=$COMMIT_HASH" >> /root/.env

RUN chmod +x /opt/pwnctl-exec/entrypoint.sh
ENTRYPOINT ["/opt/pwnctl-exec/entrypoint.sh"]