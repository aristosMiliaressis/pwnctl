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

RUN git clone https://github.com/dcsync/recontools.git /opt/pwntainer/recontools
RUN git clone https://github.com/danielmiessler/SecLists.git /opt/pwntainer/wordlists
RUN wget -O /opt/pwntainer/wordlists/commonspeak2.txt https://raw.githubusercontent.com/assetnote/commonspeak2-wordlists/master/subdomains/subdomains.txt

RUN printf ".databases\n.quit" | sqlite3 /opt/pwntainer/pwntainer.db

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY ["pwnctl/pwnctl.csproj", "pwnctl/"]
RUN dotnet restore "pwnctl/pwnctl.csproj"
COPY . .
RUN dotnet build "/pwnctl/pwnctl.csproj" -c Release -o /app/build
RUN dotnet publish "/pwnctl/pwnctl.csproj" -r linux-x64 -c Release -o /app/publish # -p:PublishSingleFile=true

FROM base AS release

RUN wget -O /usr/local/bin/job-queue.sh https://raw.githubusercontent.com/aristosMiliaressis/job-queue.sh/master/job-queue.sh \
    && chmod +x /usr/local/bin/job-queue.sh

COPY --from=build /app/publish /app

RUN ln -s /app/pwnctl /usr/local/bin/pwnctl

WORKDIR /opt

COPY workflows /app/workflows
COPY scripts /app/scripts
COPY recon_scripts /app/recon_scripts
COPY entrypoint.sh /app/entrypoint.sh

RUN mv /app/recon_scripts/resolvers_top25.txt /opt/dnsvalidator/ \
    && mv /app/recon_scripts/top200000.txt /opt/pwntainer/wordlists/ \
    && mv /app/recon_scripts/top20000.txt /opt/pwntainer/wordlists/ \
    && chmod +x /app/recon_scripts/* \ 
    && mv /app/recon_scripts/* /usr/local/bin \
    && rm -r /app/recon_scripts/ \
    && chmod +x /app/scripts/* \ 
    && mv /app/scripts/* /usr/local/bin \
    && rm -r /app/scripts/ \
    && chmod -R +x /app/workflows/
    # cat /app/aliases.txt >> /root/.bashrc

ENV INSTALL_PATH=/opt/pwntainer

RUN get_public_suffixes.sh

RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
