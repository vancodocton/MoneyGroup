#!/bin/bash

# Set environment variables
export PGPASSWORD="postgres"
export PGUSER="postgres"
export PGPORT="5432"
export PGDATABASE="postgres"

# Terminate all connections to the database
psql -e -c "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = 'MoneyGroup' AND pid <> pg_backend_pid();"

# Drop the database if it exists
psql -e -c "DROP DATABASE IF EXISTS \"MoneyGroup\";"

# Restore the database from the dump file
psql -e -f "scripts/dump.sql"
