# Northwind Applications

## Module 3. Requests to data services

### Build and Run
```sh
git clone https://github.com/yurygolub/northwind-apps-module-3.git
cd northwind-apps-module-3\ReportingApp
dotnet run
```

### Change services
use file \northwind-apps-module-3\ReportingApp\appsettings.json to configure services
set "Mode": "OData" to use OData services or "Sql" to use local database

### Create database
before using sql services you have to create database

* create database using SQL script [instnwnd.sql](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql)

* create stored procedures using this file:
\northwind-apps-module-3\Northwind.ReportingServices.SqlService\Sql scripts\dbo.CreateProcedures.sql
