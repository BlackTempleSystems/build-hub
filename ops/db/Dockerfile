FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update \
    && apt-get install -y netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

RUN dotnet tool install --global Microsoft.SqlPackage
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /src

COPY ops/db/entrypoint.sh /entrypoint.sh
COPY ops/db/wait-for-sql.sh /wait-for-sql.sh
COPY ops/db/build-dacpac.sh /build-dacpac.sh
COPY ops/db/deploy-dacpac.sh /deploy-dacpac.sh

RUN chmod +x /entrypoint.sh /wait-for-sql.sh /build-dacpac.sh /deploy-dacpac.sh

ENTRYPOINT ["/entrypoint.sh"]