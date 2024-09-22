#!/bin/bash

$env:PGPASSWORD="postgres"

psql -U postgres -p 5433 -d MoneyGroupTest -f dump.sql
