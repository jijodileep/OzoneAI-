#!/bin/bash
set -e
# Placeholder DB proving multi-DB hosting on same Postgres instance.
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
  SELECT 'ozone_catalog ready' AS status;
  -- Example empty tenant DB for local smoke (real tenants created by Super Admin).
  SELECT 'CREATE DATABASE ozone_t_demo'
  WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'ozone_t_demo')\gexec
EOSQL
