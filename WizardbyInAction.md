_Table of Contents_


# Introduction #

The following is a pretty much real-world example of how to use octalforty Wizardby in various scenarios.

# In Action #

Instead of contemplating some random database schema, we'll use an already existing one and pretend we're building the application from scratch.

The guinea-project of choice will be [Oxite](http://www.codeplex.com/oxite):

> Oxite is an open source, web standards compliant, blog engine built on ASP.NET MVC.

Oxite's database schema is available [here](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/oxite_database_schema.png).

## Laying the Foundation ##

Assuming you've already created the required directory structure and added `wizardby.exe` to your `%PATH%`, you're ready to go.

Execute the following command to get you started:

```
wizardby generate /m:Oxite
```

This will generate two files in the working directory: `Oxite.mdl` and [database.wdi](DatabaseWdiReference.md):

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/initial_files.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/initial_files.png)

Open `database.wdi` to see what's been generated:

```
deployment:
    environment development
        platform            => sqlserver
        host                => "(local)\sqlexpress"
        database            => oxite
        integrated-security => true
        
    environment staging
        platform            => sqlserver
        host                => "(local)\sqlexpress"
        database            => oxite_staging
        integrated-security => true
        
    environment production
        platform            => sqlserver
        host                => "(local)\sqlexpress"
        database            => oxite_production
        integrated-security => true
```

Assuming you have Microsoft SQL Server Express Edition instance running on your local machine, you can go create an empty database. This can be done either using `wizardby.exe` by issuing [deploy](http://code.google.com/p/octalforty-wizardby/wiki/CommandLineReference#deploy) command, or using Microsoft SQL Server Management Studio:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/create_new_database.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/create_new_database.png)

If this is not the case, alter `database.wdi` appropriately and create all necessary databases.

## Writing Your First Migration ##

To begin with, we'll start with a tiny subset of Oxite database schema:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/database_schema_subset1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/database_schema_subset1.png)

`oxite_Language` is the only table that does not have Foreign Key constraints, so it is an obvious candidate to begin with. Open up `Oxite.mdl` and add first `add table` statement:

```
migration "Oxite" revision => 1:   
    version 20090323103239:
        oxite_Language:
            LanguageID type => Guid, nullable => false, primary-key => true
            LanguageName type => AnsiString, length => 8, nullable => false
            LanguageDisplayName type => String, length => 50, nullable => false
```

As you can see, MDL syntax is pretty concise: `add table` is optional altogether, as well as `add column`. The same piece of code can be rewritten as:

```
migration "Oxite" revision => 1:   
    version 20090323103239:
        add table oxite_Language:
            add column LanguageID type => Guid, nullable => false, primary-key => true
            add column LanguageName type => AnsiString, length => 8, nullable => false
            add column LanguageDisplayName type => String, length => 50, nullable => false
```

but it's always nice to save some typing.

It's now time to test your first migration. But first let's see how the database is doing:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_info_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_info_1.png)

Fair enough: we have not yet applied any migrations, and this is precisely what Wizardby tells us. Now go to Console and execute `wizardby upgrade`. This command is essentially equivalent to

```
wizardby upgrade /mdl:Oxite
```

but it's about two times shorter.

Wizardby (as of Alpha 1) will produce the following output:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_upgrade_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_upgrade_1.png)

So far so good. Let's see what's been created:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/upgrade_result_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/upgrade_result_1.png)

Now execute `wizardby info` once more to verify that the database schema is indeed at version `20090323103239`:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_info_2.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_info_2.png)

## Building Up ##

Our first migration is successfully applied, and it's time to move on. We'll continue writing migrations and this time we'll add `oxite_User` and `oxite_UserLanguage` tables.

Execute `wizardby generate` to generate `version` element in `Oxite.mdl'

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_generate_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_generate_1.png)

Now look at the schema we're writing migrations for. `oxite_User` table has a bunch of columns, most of which have duplicate type specifications (`nvarchar(256)`, `nvarchar(128)`, etc.), so it's worth moving these to `type-aliases`:

```
migration "Oxite" revision => 1:
    type-aliases:
        type-alias PK type => Guid, nullable => false
        type-alias LongName type => String, length => 256, nullable => false
        type-alias MediumName type => String, length => 128, nullable => false
        type-alias ShortName type => String, length => 100, nullable => false
		
    version 20090323103239:
        /* Removed for brevity */
```

The migration itself is pretty straightforward:

```
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
```

The only interesting parts here are the `DefaultLanguageID references => oxite_Language` and `Username type => LongName, unique => true` statements. The `references` property will create an appropriately-named foreign key reference to `oxite_Language`, and the `unique => true` will create a unique index on `Username` column.

On to `oxite_UserLanguage` (we'll keep both these tables in one version since they together form a logical chunk which and aren't worth taking apart):

```
        ozite_UserLangage:
            UserID references => oxite_User
            LanguageID references => oxite_Language

            index "" columns => [UserID, LanguageID], unique => true, clustered => true
```

As of Alpha 1, Wizardby does not support composite primary keys, so we'll instead create a unique clustered index on `UserID` and `LanguageID` columns.

Save, execute `wizardby upgrade` and... whoops! Noticed the typo? We've created `ozite_UserLangage` instead of `oxite_UserLanguage`, which is not what we need.

To solve this problem, we'd either have to manually drop the table and then recreate it with a correct name, or modify an existing migration. Generally speaking, modifying already applied migrations is strongly discouraged, but if you really know what you're doing it's acceptable.

Execute `wizardby rollback` to undo the last migration (you can also use `wizardby migrate 20090323103239`, but that's too wordy):

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_rollback_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/wizardby_rollback_1.png)

Then correct the typo in `Oxite.mdl` and upgrade once more. You can now see that the database schema Wizardby has generated is almost identical to what Oxite's schema looks like (sans composite primary key on `oxite_UserLanguage`).

## Working Collectively ##

Suppose your colleague has also started working on the same Migration Definition. This is how to do collective development with Wizardby.

Suppose Mr. Z The Programmer decided to work on two resource-related tables: `oxite_FileResource` and `oxite_UserFileResourceRelationship`. Here's how his part of the Migration Definition looks like:

```
    /* Previous versions skipped */
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
            FileResourceID references => oxite_FileResource

            index "" columns => [UserID, FileResourceID], unique => true, clustered => true
```

And you decided to do `oxite_Role` and `oxite_UserRoleRelationship` stuff:

```
    /* Previous versions skipped */
    version 20090331135627:
        oxite_Role:
            RoleID type => PK, primary-key => true
            ParentRoleID references => oxite_Role, nullable => true			
            RoleName type => LongName

        oxite_UserRoleRelationship:
            UserID references => oxite_User
            RoleID references => oxite_Role
```

Your part is obviously simpler, so you end up committing (you _do_ use version control, don't you?) first. Now Mr. Z The Programmer has to merge your changes with his own local version of `Oxite.mdl`. Provided everything went fine, he issues `wizardby upgrade` command to apply your changes. Now Wizardby automatically detects which migrations still need to be applied and runs only these:

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/collective_development_1.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/collective_development_1.png)

Note how only `20090331135627` was applied, and your database now has all four migrations registered.