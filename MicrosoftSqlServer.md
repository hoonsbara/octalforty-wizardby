# Microsoft SQL Server #

Platform alias for Microsoft SQL Server is `sqlserver2005` for SQL Server 2005 and `sqlserver2008` for the 2008 version. Thus, it can be specified either as
```
wizardby upgrade /p:sqlserver2008
```
or, using `database.wdi`:
```
deployment:
    environment development
        platform => sqlserver2005
```

## Supported Features ##

Microsoft SQL Server platform supports pretty much everything octalforty Wizardby can do: adding, altering and removing tables and columns, working with indexes and references.

## Data Types ##

| **DbType** | **Microsoft SQL Server Type** |
|:-----------|:------------------------------|
| `AnsiString` | `varchar(max)` if `length` is not specified, `varchar(<length>)` otherwise |
| `AnsiStringFixedLength` | `char`                        |
| `Binary`   | `varbinary(max)` if `length` is not specified, `varbinary(<length>)` otherwise |
| `Boolean`  | `bit`                         |
| `Byte`     | `tinyint`                     |
| `Int32`    | `int`                         |
| `Int64`    | `bigint`                      |
| `Guid`     | `uniqueidentifier`            |
| `String`   | `nvarchar(max)` if `length` is not specified, `nvarchar(<length>)` otherwise |
| `Time`     | `rowversion`                  |