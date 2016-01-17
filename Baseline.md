# Introduction #

You don't always start with a blank slate. Often, your database already has tables that you don't want to lose, neither you want to reverse-engineer the database.

Here's where baseline comes into play.

# Baselines by Example #

Suppose your database already has several dozens tables, one of which is `tblUsers` with somewhat strange naming:

```
tblUsers
    int_id int not null primary key
    str_fullName string(200) null
    dt_dateofBirth datetime not null
```

Now, you're ready to write your first migration:

```
version 1:
    add table Profile:
        ID type => "int64!", primary-key => true
        UserID references => tblUsers
```

Naturally, Wizardby is unable to compile this migration script. It's the `UserID references => tblUsers` statement that is causing problems because Wizardby doesn't know anything about `tblUsers` table and thus cannot infer the type of a `UserID` column.

For obvious reasons you cannot include an `add table` statement for `tblUsers` in your migration:

```
version 1:
    add table tblUsers:
        int_id type => "int32!", primary-key => true
        
    add table Profile:
        ID type => "int64!", primary-key => true
        UserID references => tblUsers
```

Your database already has `tblUsers` table, but Wizardby will go ahead and try to create it, which will fail.

Baseline to rescue:

```
baseline:
    add table tblUsers:
        int_id type => "int32!", primary-key => true

version 1:        
    add table Profile:
        ID type => "int64!", primary-key => true
        UserID references => tblUsers
```

Now, reference to `tblUsers` resolves perfectly fine and version 1 of your migration is applied successfully.

Note that nothing in `baseline` element actually gets executed: tables added in said element serve only referential purpose (that is, they're neither created when upgrading database schema, nor dropped when schema is downgraded), which makes the following MDL perfectly valid from Wizardby's standpoint:

```
baseline:
    add table tblUsers:
        int_id type => "int32!", primary-key => true
        non_existentColumn type => string

version 1:        
    add table Profile:
        ID type => "int64!", primary-key => true
        UserID references => tblUsers
```

This has one consequence: you don't actually have to specify all columns in `baseline` element. Include only ones you need and you'll be just fine.

And one more thing to note: if your MDL specifies [default primary key](MdlReference#Defaults.md), it gets applied to baseline tables as well. This makes baseline even more compact:

```
defaults:
    default-primary-key ID type => "Int32!", identity => true

baseline:
    add table Users

version 1:        
    add table Profile:        
        UserID references => Users
```

Here `Users` automatically gets an `ID` column, which is then referenced by `UserID` column in `Profile` table.