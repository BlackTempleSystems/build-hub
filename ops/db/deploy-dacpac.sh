#!/usr/bin/env sh
set -eu

echo "Deploying Core.dacpac..."
sqlpackage /Action:Publish \
  /SourceFile:/tmp/dacpac/Core.dacpac \
  /TargetServerName:${MSSQL_HOST},${MSSQL_PORT} \
  /TargetDatabaseName:${BUILDHUB_CORE_DB} \
  /TargetUser:sa \
  /TargetPassword:${MSSQL_SA_PASSWORD} \
  /TargetTrustServerCertificate:True

echo "Deploying Users.dacpac..."
sqlpackage /Action:Publish \
  /SourceFile:/tmp/dacpac/Users.dacpac \
  /TargetServerName:${MSSQL_HOST},${MSSQL_PORT} \
  /TargetDatabaseName:${BUILDHUB_USERS_DB} \
  /TargetUser:sa \
  /TargetPassword:${MSSQL_SA_PASSWORD} \
  /TargetTrustServerCertificate:True

echo "Deploying IntegrationTests.dacpac..."
sqlpackage /Action:Publish \
  /SourceFile:/tmp/dacpac/IntegrationTests.dacpac \
  /TargetServerName:${MSSQL_HOST},${MSSQL_PORT} \
  /TargetDatabaseName:${BUILDHUB_TEST_DB} \
  /TargetUser:sa \
  /TargetPassword:${MSSQL_SA_PASSWORD} \
  /TargetTrustServerCertificate:True

echo "Database deployment complete."