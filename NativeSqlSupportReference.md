# Introduction #

Granted, MDL is not expressive enough (and was not intended to be such) to handle each and every possible situation one may encounter while integrating a database schema. To resolve this problem, Wizardby has support for so-called "Native SQL".

The essence is simple: Wizardby allows you to write SQL statements in platform-specific dialect of SQL, keep them in files in a well-known directory structure and then reference them from an MDL file.

This reference contains all information on working with Native SQL in Wizardby.

# Working with Native SQL #

## Enabling Native SQL Support ##

By default, Wizardby does not create directory structure for Native SQL scripts. To enable this feature, open `database.wdi` and set `create-native-sql-directories` property to `true`:

```
deployment:
    environment development
        platform            => sqlserver2005
        create-native-sql-directories => true
```

After this property has been added, every time a new version is added to an MDL file with `wizardby g` command, Wizardby will create a subdirectory (in a directory named after database platform alias) for each migration version. This directory will contain two files, `upgrade.sql` and `downgrade.sql`. You are free to delete, rename and generally do anything with these files as they are created for your convenience only.

Suppose you have the following directory structure:

```
D:\
  + Acme.Project
    + src
    + db
      acme.mdl
      database.wdi
```

Assuming your `platform` is `sqlserver2005`, after enabling `create-native-sql-directories` property and executing `wizardby g` command, your directory layout will look somewhat similar to:

```
D:\
  + Acme.Project
    + src
    + db
      + sqlserver2005
        + 20091205143122
          downgrade.sql
          upgrade.sql
      acme.mdl
      database.wdi
```

## Shared Native SQL Scripts ##

If you want to use certain SQL script for all platforms, drop it into `shared\<version-number>` directory:

```
D:\
  + Acme.Project
    + src
    + db
      + shared
        + 20091205143122
          downgrade.sql
          upgrade.sql
      acme.mdl
      database.wdi
```

This way, `upgrade.sql` will be executed for every platform.

## Referencing Native SQL Scripts from an MDL File ##

See [MDL Reference](MdlReference#Executing_Native_SQL.md) for information on how to reference these files from an MDL file.

## Writing Native SQL Scripts ##

There is a number of things to consider when writing Native SQL scripts:

  * Try not to use nested transactions since they break the natural transaction flow Wizardby expects to be there
  * Use `go` to separate statements in a batch