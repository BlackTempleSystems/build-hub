#!/usr/bin/env sh
set -eu

echo "Waiting for SQL Server at ${MSSQL_HOST}:${MSSQL_PORT}..."

until nc -z "${MSSQL_HOST}" "${MSSQL_PORT}"
do
  sleep 2
done

echo "SQL Server port is reachable."