$MSSQL_SA_PASSWORD="password123!"

# Terminate all connections to the database
sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -Q "ALTER DATABASE [MoneyGroup] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;"

# Drop the database if it exists
sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -Q "DROP DATABASE IF EXISTS MoneyGroup;"

# Restore the database from the dump file
sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i .\scripts\1.create-database.sql -i .\scripts\2.migrations.sql -i .\scripts\3.dump.sql
