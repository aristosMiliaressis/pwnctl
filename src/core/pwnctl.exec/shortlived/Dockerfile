FROM ubuntu AS tool-build

RUN apt-get update \
    && apt-get install -y git make unzip musl-dev gcc curl

# Setting up Go lang
COPY --from=golang:1.20 /usr/local/go/ /usr/local/go/
ENV GOROOT "/usr/local/go"
ENV GOPATH "/root/go"
ENV PATH "$PATH:$GOPATH/bin:$GOROOT/bin"

RUN go install github.com/tomnomnom/anew@latest
RUN go install github.com/tomnomnom/unfurl@latest
RUN go install github.com/projectdiscovery/tlsx/cmd/tlsx@latest
RUN go install github.com/projectdiscovery/httpx/cmd/httpx@latest
RUN go install github.com/projectdiscovery/cdncheck/cmd/cdncheck@latest
RUN go install github.com/aristosMiliaressis/urgo@latest
RUN go install github.com/rverton/webanalyze/cmd/webanalyze@latest
RUN go install github.com/sensepost/gowitness@latest
RUN go install github.com/zmap/zdns@latest
RUN go install github.com/projectdiscovery/notify/cmd/notify@latest

WORKDIR /tmp

# Installing private tools
RUN mkdir -p /opt/wordlists/dns/
RUN mkdir -p -m 0700 ~/.ssh && ssh-keyscan github.com >> ~/.ssh/known_hosts
RUN --mount=type=ssh \
    git clone -b pwnctl git@github.com:aristosMiliaressis/sub-enum.git \
    && cp sub-enum/wordlists/* /opt/wordlists/dns/


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
COPY ["/pwnctl.exec/shortlived/pwnctl.exec.shortlived.csproj", "/src/pwnctl.exec/shortlived/"]
RUN dotnet restore "/src/pwnctl.exec/shortlived/pwnctl.exec.shortlived.csproj"

COPY . /src

RUN dotnet publish "/src/pwnctl.exec/shortlived/pwnctl.exec.shortlived.csproj" -c Release -o /src/publish


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

# Setting up python
RUN apt-get update \
    && apt-get install -y python3 \
    python3-pip \
    python3-setuptools
RUN cp /usr/bin/python3 /usr/bin/python

# Setting up xvfb for headless browsing
WORKDIR /opt/chromium

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgbm1 libxkbcommon0 \
    xvfb gconf-service libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 \
    libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 \
    libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 \
    libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 \
    ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils curl nmap git

# Installing tools
RUN mkdir /opt/tools
WORKDIR /opt/tools

RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y jq whois dnsutils ldnsutils less

RUN python3 -m pip install useragent-picker-cli && uagen
RUN python3 -m pip install s3scanner
RUN python3 -m pip install emailfinder
RUN python3 -m pip install wafw00f==v2.2.0
RUN python3 -m pip install -e git+https://github.com/brandonscholet/python-Wappalyzer.git#egg=python-Wappalyzer
RUN python3 -m pip install -e git+https://github.com/brandonscholet/wappybird.git#egg=wappybird \
    && python3 -m pip install click validators
RUN git clone https://github.com/punk-security/dnsReaper.git && cd dnsReaper \
    && python3 -m pip install -r requirements.txt
RUN curl https://github.com/mgdm/htmlq/releases/download/v0.4.0/htmlq-x86_64-linux.tar.gz -L -o htmlq-x86_64-linux.tar.gz \
    && tar xzf htmlq-x86_64-linux.tar.gz \
    && rm htmlq-x86_64-linux.tar.gz && mv htmlq /usr/local/bin

COPY --from=tool-build /root/go/bin/ /usr/local/bin/
COPY --from=tool-build /opt/wordlists/ /opt/wordlists/
COPY --from=worker-build /src/publish /opt/pwnctl-exec
COPY pwnctl.exec/wordlists/ /opt/wordlists/
COPY pwnctl.exec/scripts/* /opt/scripts/
COPY pwnctl.exec/shortlived/entrypoint.sh /opt/pwnctl-exec/entrypoint.sh

RUN chmod -R +x /opt/scripts \
    && mv /opt/scripts/* /usr/local/bin/

ARG COMMIT_HASH
RUN echo "export PWNCTL_COMMIT_HASH=$COMMIT_HASH" >> /root/.profile

RUN chmod +x /opt/pwnctl-exec/entrypoint.sh
ENTRYPOINT ["/opt/pwnctl-exec/entrypoint.sh"]