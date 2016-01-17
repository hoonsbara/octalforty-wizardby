# Introduction #

Pretty much inspired by Ruby on Rails' Migrations, octalforty Wizardby is a powerful yet easy to use database migration framework primarily targeting .NET projects.

### What is a Migration ###

Migrations are very simple in their essence. They provide means to specify changes to a database schema and to apply them in a controlled, structured and consistent fashion. Database schema can be altered manually, using hand-written SQL DDL scripts, but then you immediately become responsible for applying them in an orderly manner, for controlling which versions were applied and which were not, for communicating with other developers, for writing downgrade migrations for, as well as writing migrations for various DB platforms, if this is the case. Versioning multiple database instances (think development, staging and production databases) is no easy task when done manually.

Wizardby helps you solve most (if not all) these problems. Specifically, it allows you to write migrations using concise, DRY, platform-independent and human-readable DSL; it also tracks which migrations were applied to a particular instance of a database and automatically upgrades database schema. It also generates downgrade scripts, so you can always revert your database schema to an arbitrary version.

Here's how a typical migration definition looks like:
```
migration "Blog" revision => 1:
    type-aliases:
        type-alias N type => String, length => 200, nullable => false, default => ""

    defaults:
        default-primary-key ID type => Int32, nullable => false, identity => true

    version 20090226100407:
        add table Author: /* Primary Key is added automatically */
            FirstName type => N /* “add” can be omitted */
            LastName type => N
            EmailAddress type => N, unique => true /* "unique => true" will create UQ_EmailAddress index */
            Login type => N, unique => true
            Password type => Binary, length => 64, nullable => true

            /* The following will create an index which will be called UQ_LoginEmailAddress */
            index "" unique => true, columns => [[Login, asc], EmailAddress]

        Tag: /* "add table" is optional as well */
            Name type => N

        Blog:
            Name type => N
            Description type => String, nullable => false

        BlogPost:
            Title type => N
            Slug type => N
            BlogID references => Blog /* Column type is inferred automatically */
            AuthorID: 
                reference pk-table => Author

        add table BlogPostTagJunction primary-key => false: /* No PK here */
            BlogPostID references => BlogPost
            TagID references => Tag

    version 20090226100408:
        add table BlogPostComment:
            BlogPostID references => BlogPost
            AuthorEmailAddress type => N
            Content type => String, nullable => false
            
    version 20090316120423:
        alter table BlogPostComment:
            add column PublishedOn type => DateTime
            
    version 20090317223243:
        remove table BlogPostComment            
```

# What's Next? #

Continue to [Wizardby In Action](WizardbyInAction.md), [MDL Reference](MdlReference.md) and then [Command-Line Reference](CommandLineReference.md).