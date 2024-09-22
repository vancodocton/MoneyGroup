#!/bin/bash

export PGPASSWORD=postgres

psql -U postgres -p 5433 -d MoneyGroupTest -f dump.sql
