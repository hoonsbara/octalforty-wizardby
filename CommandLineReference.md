_Table of Contents_


# Introduction #

Wizardby comes with a command-line tool, `wizardby.exe`.

# Reference #

## Overview ##

General pattern for invoking `wizardby.exe` is as follows:

```
    wizardby <command> <version-or-step> [/mdl:<mdl-file>] [/connection:<connection-string>] [/platform:<platform-alias>] 
        [/environment:<environment-name>]
```

As you see, only `command` and `version-or-step` arguments are required ones, other can be freely omitted as Wizardby relies on [Convention over Configuration](http://en.wikipedia.org/wiki/Convention_over_configuration) principle in this respect.

### `mdl` ###

The `mdl` argument specifies the name of the migration definition. If it is omitted, Wizardby will search for the MDL file in current directory and use it.

### `connection` ###

This argument specifies the connection string to be used by Wizardby. If it is omitted, the connection string is built up using [database.wdi](DatabaseWdiReference.md) found in the current directory.

### `platform` ###

This argument specifies the platform alias, and if omitted uses one from the `database.wdi`.

### `environment` ###

This argument specifies an environment name from `database.wdi`. `environment` defaults to `development`.

## Commands ##

### Migration Commands ###

#### `migrate` ####

`migrate` (or `m` for short) is the workhorse of Wizardby. It migrates database schema to the specified version, or downgrades the database altogether if `version-or-step` equals to `0`. Note that it either upgrades or downgrades database schema depending on current version.

In other words, `wizardby m 20090430` will migrate (i.e. downgrade or upgrade) the database schema to version `20090430`, whereas `wizardby m 0` will downgrade database schema to version `0`.

#### `upgrade` ####

`upgrade` (or `u` for short) will apply all outstanding migrations and upgrade the database schema to the latest revision. This is equivalent to running `wizardby m xxx` with `xxx` being the most recent migration version.

#### `downgrade` ####

`downgrade` (or `do` for short; note that specifying `d` will cause an error) will effectively migrate database schema to version `0`. This is equivalent of running `wizardby m 0`.

#### `rollback` ####

`rollback` or (`ro` for short) will undo the most recent version, or, if `version-or-step` argument is specified, it will undo the given number of migrations.

Assume you have this migration definition:

```
migration "Oxite" revision => 1:
    type-aliases:
        type-alias PK type => Guid, nullable => false
        type-alias LongName type => String, length => 256, nullable => false
        type-alias MediumName type => String, length => 128, nullable => false
        type-alias ShortName type => String, length => 100, nullable => false
		
    version 20090323103239:
        oxite_Language:
            LanguageID type => Guid, nullable => false, primary-key => true
            LanguageName type => AnsiString, length => 8, nullable => false
            LanguageDisplayName type => String, length => 50, nullable => false

    version 20090330170528:
        oxite_User:
            UserID type => PK, primary-key => true
            Username type => LongName, unique => true
            DisplayName type => LongName
            Email type => LongName
            HashedEmail type => ShortName
            Password type => MediumName
            PasswordSalt type => MediumName
            DefaultLanguageID references => oxite_Language
            Status type => Byte, nullable => false
            
        oxite_UserLanguage:
            UserID references => oxite_User
            LanguageID references => oxite_Language

            index "" columns => [UserID, LanguageID], unique => true, clustered => true

    version 20090331135627:
        oxite_Role:
            RoleID type => PK, primary-key => true
            ParentRoleID references => oxite_Role, nullable => true			
            RoleName type => LongName

        oxite_UserRoleRelationship:
            UserID references => oxite_User
            RoleID references => oxite_Role	

    version 20090331140131:
        oxite_FileResource:
            FileResourceID type => PK, primary-key => true
            SiteID type => Guid, nullable => false
            FileResourceName type => LongName
            CreatorUserID references => oxite_User
            Data type => Binary
            ContentType type => AnsiString, length => 25, nullable => false
            Path type => String, length => 1000, nullable => false
            State type => Byte, nullable => false
            CreatedDate type => DateTime, nullable => false
            ModifiedDate type => DateTime, nullable => false 

        oxite_UserFileResourceRelationship:
            UserID references => oxite_User
            FileResourceID references => oxite_FileResource:
                add index unique => true

            index "" columns => [UserID, FileResourceID], unique => true, clustered => true      
```

and your database is at version `20090331140131`. Running `wizardby ro` will revert version `20090331140131` and your database will be at version `20090331135627`. If you then run `wizardby ro 2`, this will roll back two more versions leaving your database at version `20090323103239`. Issuing `wizardby ro 2` once more will totally downgrade your database to version `0`: note that Wizardby rolls back only one version despite `2` was requested.

#### `redo` ####

This command (short version is `red`, which is just not worth saving) will reapply a given number of migrations (or only last one, if `version-or-step` argument is not specified).

Given the migration definition from above and assuming your database version is the most recent one, running `wizardby re 3` will rollback 3 last versions, and then reapply them again.

### Informational Commands ###

#### `info` ####

This command will inspect a given database and print out current database version and all applied migrations:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_info_2.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_info_2.png)

### Generational Commands ###

#### `generate` ####

This command (a shorthand `g` will do) has two meanings: it will either generate a `MDL`-`WDI` pair if none of these exist (remember to specify `/m` argument), or generate a new `version` statement in an existing MDL file:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_generate_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_generate_1.png)

#### `reverseengineer` ####

This command (use `rev` to save some typing) will inspect a given database and as of Alpha 2 will produce a `baseline.mdl`, which will contain code to recreate your database from scratch:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_reverseengineer_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_reverseengineer_1.png)

### Deployment Commands ###

#### `deploy` ####

This command (`de` for short) will create a database, but will not apply any migrations. Remember that creating a database isn't that simple and Wizardby aims for greatest common denominator of all supported platforms, so this command should be used in development environments only.