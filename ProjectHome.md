# octalforty Wizardby #
This is octalforty Wizardby, a powerful yet easy to use database continuous integration & schema migration framework primarily targeting .NET. The latest release is [Alpha 3](http://code.google.com/p/octalforty-wizardby/downloads/list).

## What Is Wizardby? ##

Managing changes to a database schema has been one of the most complex tasks for a team of developers. Most have relied on storing platform-specific DDL in revision control, manually applying them for each deployment on each and every database instance.

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/database_versioning_without_wizardby.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/database_versioning_without_wizardby.png)

Downsides of this approach are obvious:

  * All change scripts must be written manually, with all the duplication this incurs
  * Information about applied scripts has to be tracked manually as well
  * Rollbacks are impossible unless rollback scripts are written -- manually, of course
  * Manual transaction management, which can easily leave database in an inconsistent state

Wizardby alleviates all these problems.

![http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/database_versioning_with_wizardby.png](http://octalforty-wizardby.googlecode.com/svn/trunk/docs/img/database_versioning_with_wizardby.png)

  * Single platform-independent script, which is _the_ authoritative source for the database schema. You don't have to worry about precise syntax of `alter table` now
  * Wizardby automatically keeps track of applied migrations. Applying all changes to a database becomes as easy as executing `wizardby upgrade` on the command line; migrate to specific version with `wizardby migrate 20090430`.
  * Rollback scripts are generated completely automatically
  * Transactions are managed automatically as well; your databases will always be in a consistent state
  * Wizardby can easily be integrated into your build and deployment process or can be used inside your application

## Why Versioning My Database Is Important? ##

For a comprehensive discussion of this topic, read a series of posts by K. Scott Allen:

  * [Three Rules for Database Work](http://odetocode.com/blogs/scott/archive/2008/01/30/three-rules-for-database-work.aspx)
  * [Versioning Databases – The Baseline](http://odetocode.com/blogs/scott/archive/2008/01/31/versioning-databases-the-baseline.aspx)
  * [Versioning Databases – Change Scripts](http://odetocode.com/blogs/scott/archive/2008/02/02/versioning-databases-change-scripts.aspx)

And see [what users are saying](Testimonials.md).

## Features ##

octalforty Wizardby provides a simple and DRY DSL for writing migrations in database-independent manner.

  * Database-independent [DSL](MdlReference.md) for writing migrations
  * Supports collective development
  * Automatic [database deployment](http://code.google.com/p/octalforty-wizardby/wiki/CommandLineReference#deploy)
  * Automatic [schema version tracking](WizardbyInAction.md)
  * Automatic generation of [downgrade scripts](WizardbyInAction.md)
  * [Schema reverse engineering](http://code.google.com/p/octalforty-wizardby/wiki/CommandLineReference#reverseengineer)
  * Supports [several database platforms](SupportedDbPlatforms.md)
  * Supports [Native SQL Scripts](NativeSqlSupportReference.md)
  * Continuous Integration-ready
  * [Frictionless](CommandLineReference.md) development flow
  * Powerful [Developer API](DeveloperApi.md)

## Getting Started ##

### Downloading ###

Current version of octalforty Wizardby can be downloaded [here](http://code.google.com/p/octalforty-wizardby/downloads/list). Both binaries and source code distributions available. You might also [browse](http://code.google.com/p/octalforty-wizardby/source/browse/) Subversion repository or [check out](http://code.google.com/p/octalforty-wizardby/source/checkout) source altogether.

### Documentation ###

Refer to the [Getting Started](GettingStarted.md) page for information on what is a migration and how Wizardby can help you. See [MDL Reference](MdlReference.md) for information on how to write migration definitions, and [Wizardby In Action](WizardbyInAction.md) to see how to use Wizardby in real-life development.

See [Supported DB Platforms](SupportedDbPlatforms.md) page for a list of supported DB platforms:

  * [Microsoft SQL Server](MicrosoftSqlServer.md) 2005 & 2008
  * [Microsoft SQL Server Compact Edition](MicrosoftSqlCe.md)
  * [SQLite](SQLite.md)