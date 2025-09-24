#!/bin/bash

# Set environment variables
export PGPASSWORD="postgres"
export PGUSER="postgres"
export PGPORT="5432"
export PGDATABASE="MoneyGroup"

# Run pg_dump command
pg_dump -C -F p
