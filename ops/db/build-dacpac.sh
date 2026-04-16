#!/usr/bin/env sh
set -eu

echo "Building database projects..."

cd /src/Databases
dotnet restore Databases.slnx
dotnet build Databases.slnx -c Release --no-restore

mkdir -p /tmp/dacpac
find /src/Databases -type f -name "*.dacpac" -exec cp {} /tmp/dacpac/ \;

echo "Built DACPACs:"
ls -lah /tmp/dacpac