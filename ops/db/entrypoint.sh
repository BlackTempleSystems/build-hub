#!/usr/bin/env sh
set -eu

/wait-for-sql.sh
/build-dacpac.sh
/deploy-dacpac.sh