#!/bin/bash

export PGPASSWORD=postgres

pg_dump -U postgres -p 5433 -d MoneyGroupTest -F p -c --exclude-table-data='"__EFMigrationsHistory"' > dump.sql
