$env:PGPASSWORD="postgres"
$env:PGUSER="postgres"
$env:PGPORT="5432"
$env:PGDATABASE="MoneyGroup"

pg_dump -C -F p
