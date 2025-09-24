#!/bin/bash

# Terminate all connections to the database
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -Q "ALTER DATABASE [MoneyGroup] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;"

# Drop the database if it exists
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -Q "DROP DATABASE IF EXISTS MoneyGroup;"

# Restore the database from the dump file
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i ./1.create-database.sql -i ./2.migrations.sql -i ./3.dump.sql
