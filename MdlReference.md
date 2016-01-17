_Table of Contents_


# Introduction #

All migrations are specified in an MDL file, which is a plain-text file. The syntax of a Migration Definition Language if somewhat similar to Python (indentation and colons are used to delimit blocks) and Ruby. Here’s a glimpse of what a typical MDL file looks like:

```
migration "Blog" revision => 1:
    type-aliases:
        type-alias N type => "String!(200)", default => ""

    defaults:
        default-primary-key ID type => Int32, nullable => false, identity => true

    version 20090226100407:
        add table Author: /* Primary Key is added automatically */
            FirstName type => N /* “add” can be omitted */
            LastName type => N
            EmailAddress type => N, unique => true /* "unique => true" will create UQ_EmailAddress index */
            Login type => N, unique => true
            Password type => Binary, length => 64, nullable => true

            index UQ_LoginEmailAddress unique => true, columns => [[Login, asc], EmailAddress]

        add table Tag:
            Name type => N

        add table Blog:
            Name type => N
            Description type => String, nullable => false

        add table BlogPost:
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
```

In the most basic scenario a MDL file contains a migration block with a bunch of nested version blocks. Those version blocks contains statements which describe what you want to be done to your database schema.

## Brief Syntax Overview ##

MDL syntax is pretty straightforward. Every statement and block starts with an identifier (be it a keyword or a symbol), nested blocks are indented and are preceded by a colon in the end of a parent statement.

```
/* This is a valid block */
version 20090226100408:
    add table Foo

/* This is invalid block: note missing semicolon */
version 20090226100408
    add table Foo

/* This is also an invalid block: note wrong indentation on the second line */
version 20090226100408:
add table Foo
```
Every statement may contain properties, which consist of a property name, property assignment (`=>`) and property value. Values can be symbols, strings or integer constants:

```
add column Bar type => String, length => 200, nullable => false, unique => “true”
```

### Lists ###

Lists are used in several places, so it's better to know about them to. In its essence, a lists is a collection of comma-delimited "tokens" enclosed in square brackets:

```
[This, "is", a, list]
```

"Token" here means a symbol, a string or integer constant, or another list, so this is perfectly legal (though not much used):

```
[This, is, ["an", ["example", of], nested], lists]
```

# Writing Migrations #

## Migration ##

Every MDL file must begin with a `migration` statement, which must contain the name of the migration and the `revision` property, which indicates the version of an MDL language, which is `1` for the time being:

```
migration "Sample Migration" revision => 1:
```

## Getting Dry ##

### Specifying Types ###

Wizardby uses type names from standard .NET [DbType](http://msdn.microsoft.com/en-us/library/system.data.dbtype.aspx) enumeration.

Since [r153](http://code.google.com/p/octalforty-wizardby/source/detail?r=153) ([issue 20](https://code.google.com/p/octalforty-wizardby/issues/detail?id=20)) type names can be specified in a case-insensitive manner.

Since [r154](http://code.google.com/p/octalforty-wizardby/source/detail?r=154) ([issue 21](https://code.google.com/p/octalforty-wizardby/issues/detail?id=21)) type specifications can be written in a much more concise manner:

```
add column Fizz type => string, nullable => true
add column Foo type => string, length => 200, nullable => false
add column Bar type => decimal, nullable => true, scale => 18, precision => 2
```

can be replaced with:

```
add column Fizz type => "string?"
add column Foo type => "string!(200)"
add column Bar type => "decimal?(18, 2)"
```

The pattern is as follows:

  * Mandatory type name
  * Mandatory nullability specification (`?` for nullable, `!` for non-nullable)
  * Either optional length
  * ...or scale and precision

Note that this shortcut declaration has to be enclosed in quotes: `type => decimal?(18, 2)` won't work.

### Type Aliases ###

Being a [DRY](http://en.wikipedia.org/wiki/DRY) language, MDL allows you to specify shortcuts for types, which will save you some typing later on and will in fact maintain DRYness.

In order to define a type alias, you need to put a `type-aliases` block as a child of `migration` statement and then add `type-alias` statements for each type alias:

```
migration "Sample Migration" revision => 1:
    type-aliases:
        type-alias NI32 type => Int32, nullable => true
```

Allowed properties for `type-alias` statement are `type`, `length`, `nullable`, `scale` and `precision`.

`type-alias` keyword is optional.

Note that you can override standard `DbType` enumeration members by specifying a type alias with the same name:

```
type-alias Int32 type => Int64, nullable => false, length => 4
```

Moreover, type aliases can "inherit" from one another:

```
type-alias I32 type => Int32, nullable => false
type-alias NI32 => type => I32, nullable => true
```

### Defaults ###

MDL allows for specification of certain defaults. As of [revision 1](https://code.google.com/p/octalforty-wizardby/source/detail?r=1), only `default-primary-key`, which specifies attributes of a Primary Key, is supported.

```
migration "Sample Migration" revision => 1:
    defaults:
        default-primary-key ID type => Int32, nullable => false, identity => true
```

Allowed properties for `default-primary-key` statement are `type`, `length`, `nullable`, `default`, `scale`, `precision` and `identity`.

Note that `type` property may refer to a previously defined type alias:

```
migration "Sample Migration" revision => 1:
    type-aliases:
        type-alias I32 type => Int32, nullable => false

    defaults:
        default-primary-key ID type => I32, identity => true
```

### Templates ###

Templates allow you to define common portions of tables and reuse them later on.

```
migration "Sample Migration" revision => 1:
    templates:
        table template WithSoftDelete:
            add column IsDeleted type => Boolean
```

See "Adding Columns" section on `add column` syntax.

## Baseline ##

You don't always start with an empty slate. Sometimes you have a legacy database, and `baseline` allows you to describe its schema. See [this document](Baseline.md) for a more in-depth discussion of baselines.

```
migration "Sample Migration" revision => 1:
    baseline:
        add table tblUsers: /* See how legacy it is :) */
            tblUserID type => String, length => 10, nullable => false, primary-key => true
```

Note that `add` keyword can be freely omitted.

See "Adding Tables" section on how to write `add table` statements.

## Version ##

`version` statement is the workhorse of Wizardby. It must contain nothing but a unique version number. You'd be better off if you'll use timestamps as version numbers:

```
migration "Sample Migration" revision => 1:
    version 20090226100408:
```

### Adding Tables ###

Tables are, quite naturally, added using `add table` statement, which specifies the name of the table being created:

```
migration "Sample Migration" revision => 1:
    defaults:
        default-primary-key ID type => Int32, nullable => false, identity => true

    version 20090226100408:
        add table Author
```

This will create an `Author` table that will contain a single `ID` column, as defined in the `default-primary-key` statement.

`add table` keywords are totally optional, since this is the default behavior.

#### Specifying Primary Key ####

When [default-primary-key](http://code.google.com/p/octalforty-wizardby/wiki/MdlReference#Defaults) is specified, each table receives a Primary Key column as defined in the `default-primary-key` element. If you want to override this behavior or do not want to include a Primary Key altogether (for example in case of a junction table for many-to-many relationship), add `primary-key => false` property to the `add table` statement:

```
    version 20090226100408:
        add table BlogAuthorJunction primary-key => false:
            BlogID references => Blog
            AuthorID references => Author
```

Primary Key can be specified manually by adding `primary-key => true` property to the `add column` statement:

```
    version 20090226100408:
        add table Tag primary-key => false:
            TagID type => Int32, primary-key => true
```

See next section on `add column` syntax.

#### Using Templates ####

[Templates](http://code.google.com/p/octalforty-wizardby/wiki/MdlReference#Templates) allow you to move "shared" `add column` definitions to a well-known location and then reference them in `add table` statements. In order to reference a Template, add `templates` property to the `add table` statement with value being a [list](http://code.google.com/p/octalforty-wizardby/wiki/MdlReference#Lists) of template names:

```
    version 20090226100408:
        add table Author templates => [WithSoftDelete]:
            FirstName type => String, length => 2000, nullable => false
```

All columns from templates are added after columns which are explicitly specified in the `add table` block.

As of [r174](http://code.google.com/p/octalforty-wizardby/source/detail?r=174) ([issue 22](https://code.google.com/p/octalforty-wizardby/issues/detail?id=22)) there's a way to explicitly specify, where exactly should the template be included:

```
    version 20090226100408:
        add table Author:
            include-template WithSoftDelete
            FirstName type => "String!(2000)"
```

See [Specifying Types](MdlReference#Specifying_Types.md) section on explanation for the `String!(2000)` construct.

### Adding Columns ###

Columns are added using `add column` statement. It specifies the name of the column and its various properties (type, length, nullability, etc.):

```
    version 20090226100408:
        add table Author:
            add column EmailAddress type => String, length => 200, nullable => false
```

When `add column` statement is used inside an `add table` block, `add column` keywords can be skipped:

```
    version 20090226100408:
        Author:
            EmailAddress type => String, length => 200, nullable => false
```

To add column to an existing table, you should add `add column` statement to `alter table` block (see "Alter Table").

Properties of an `add column` statement:

| **Property** | **Value** |
|:-------------|:----------|
| `type`       | `DbType` of the column |
| `length`     | Integer constant; length of the column (if applicable) |
| `nullable`   | Boolean (`true` or `false`); nullability of the column |
| `scale`      | Integer constant; scale of the column (if applicable) |
| `precision`  | Integer constant; precision of the column (if applicable) |
| `default`    | Default expression for the column (platform-specific) |
| `primary-key` | Boolean value; primary key flag |
| `identity`   | Boolean value; indicates whether this is an identity column |
| `unique`     | Boolean value; if `true` creates a unique index for this column |
| `references` | When present, creates a foreign-key reference to the table |

### Adding Indexes ###

To add an index, use an `add index` statement. There are several variations in adding indexes.

First, if you add a `unique` property to an `add column` statement this will create a unique index for that column.

Second, if you add an `add index` statement inside an `add column` block, you can omit an otherwise required `table` and `column` properties:

```
    add table Bar:
        add column Foo:
            add index "UQ_Foo" unique => true, clustered => true
```

This will create an unique clustered index `UQ_Foo` on `Bar` table for `Foo` column. Note how an index name is enclosed in double quotes. This is required if you're using `add index` inside an `add column` block. If you leave out the index name, Wizardby will give an appropriate name automatically. And finally, `add` keyword here is optional.

Third, you can use `add index` inside an `add table` or an `alter table` statement. You won't have to specify `table` property, but you'll have to specify `column` or `columns` properties:

```
    add table Bar:
        Foo type => Int32
        Doo type => Int64

        add index "IX_Foo" column => Foo
        add index "" columns => [Foo, [Doo, desc]]
```

Note that index name in the second `add index` statement is an empty string. This forces Wizardby to name an index automatically. Also note the syntax of `columns` property value.

When using `add index` inside an `add table` block `add` in `add index` can be omitted.

And finally, `add index` can be used inside a `version` block. The syntax is all the same, but you will have to specify table name using `table` property.

### Adding References ###

References are added using `add reference` statement. Just like with `add index`, there are several options when adding references.

First, adding a `references` property to the `add column` statement will automatically add a foreign key reference to that table:

```
    add table Bar:
        ID type => Int32, primary-key => true
        Foo references => Bar
```

This will add a reference from `Foo` to the primary key column of `Bar` table.

Second, `add reference` can be added to the `add column` block. This allows you to omit `fk-table` and `fk-column` properties.

Third, when used inside an `add table` or `alter table` block, `fk-table` can be omitted.

And finally, when used inside a `version` block, all properties must be specified (note that `fk-column` and `fk-columns` as well as `pk-column` and `pk-columns` are mutually exclusive.

| **Property** | **Value** |
|:-------------|:----------|
| `pk-table`   | Specifies the name of the primary key table |
| `pk-column`  | Specifies the name of the primary key column. If omitted, defaults to the primary-key column of the `pk-table` |
| `pk-columns` | Specifies the list of primary key column names |
| `fk-table`   | Specifies the name of the foreign key table |
| `fk-column`  | Specifies the name of the foreign key column |
| `fk-columns` | Specifies the list of names of the foreign key columns |

### Altering Tables ###

Tables are altered using `alter table` statement:

```
    alter table Foo:
        /* Altering the table */
```

### Altering Columns ###

Use `alter column` to alter an existing column. This statement can only be used inside an `alter table` block. Refer to "Adding Columns" for the list of properties. Note, however, that not every property can be altered: your DB will complain if it does not support certain behavior.

### Removing Tables ###

Tables can be removed with `remove table` statement. Note that dependencies are _not_ removed.

### Removing Columns ###

Columns are removed using `remove column` statement, which can only be used inside an `alter table` block.

### Removing Indexes ###

Remove indexes with `remove index` statement. It generally requires `table` property which specifies the name of the table from which an index is removed, but when used inside an `alter table` block this property is optional:

```
    alter table Foo:
        remove index IX_Foo

    /* This is the same as above */
    remove index IX_Foo table => Foo
```

### Removing References ###

References are removed using `remove reference` statement. It requires `fk-table` property to find out the name of the foreign key table, but when used inside an `alter table` block the value of this property defaults to the name of the table being altered:

```
    alter table Foo:
        remove reference FK_Duck

    /* Equivalent to */
    remove reference FK_Duck fk-table => Foo
```

### Executing Native SQL ###

Native SQL is executed using `execute native-sql` statement. It may contain both `upgrade-resource` and `downgrade-resource` properties, or any one of these.

Values of these propertues are names of resources (basically, names of SQL files located in appropriate [subdirectory](NativeSqlSupportReference.md)) to be included in upgrade/downgrade:

```
    version 20091205143122:
        execute native-sql upgrade-resource => upgrade, downgrade-resource => downgrade
```

This statement will make Wizardby execute `upgrade.sql` while upgrading to version `20091205143122` and `downgrade.sql` when downgrading from version `20091205143122`. Note that you don't have to specify the `.sql` extension -- just file names.