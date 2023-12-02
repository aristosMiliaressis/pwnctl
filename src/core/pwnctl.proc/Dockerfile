FROM ubuntu AS tool-build

RUN apt-get update \
    && apt-get install -y ca-certificates

# Setting up Go lang
COPY --from=golang:1.20 /usr/local/go/ /usr/local/go/
ENV GOROOT "/usr/local/go"
ENV GOPATH "/root/go"
ENV PATH "$PATH:$GOPATH/bin:$GOROOT/bin"

RUN go install github.com/projectdiscovery/notify/cmd/notify@latest


FROM ubuntu AS base

# Installing system,build,network utils
RUN apt-get update \
    && apt-get install -y apt-utils \
    curl \
    git \
    groff \
    jq \
    libpcap-dev \
    moreutils \
    openssl

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

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# building proc service
COPY ["/pwnctl.kernel/pwnctl.kernel.csproj", "/src/pwnctl.kernel/"]
RUN dotnet restore "/src/pwnctl.kernel/pwnctl.kernel.csproj"
COPY ["/pwnctl.domain/pwnctl.domain.csproj", "/src/pwnctl.domain/"]
RUN dotnet restore "/src/pwnctl.domain/pwnctl.domain.csproj"
COPY ["/pwnctl.app/pwnctl.app.csproj", "/src/pwnctl.app/"]
RUN dotnet restore "/src/pwnctl.app/pwnctl.app.csproj"
COPY ["/pwnctl.infra/pwnctl.infra.csproj", "/src/pwnctl.infra/"]
RUN dotnet restore "/src/pwnctl.infra/pwnctl.infra.csproj"
COPY ["/pwnctl.proc/pwnctl.proc.csproj", "/src/pwnctl.proc/"]
RUN dotnet restore "/src/pwnctl.proc/pwnctl.proc.csproj"

COPY . src/

RUN dotnet publish "/src/pwnctl.proc/pwnctl.proc.csproj" -c Release -o /publish/proc

FROM base AS release

COPY --from=build /publish/proc /opt/pwnctl-proc
COPY --from=tool-build /root/go/bin/ /usr/local/bin/

COPY pwnctl.proc/entrypoint.sh /opt/pwnctl-proc/entrypoint.sh
RUN chmod +x /opt/pwnctl-proc/entrypoint.sh
ENTRYPOINT ["/opt/pwnctl-proc/entrypoint.sh"]
