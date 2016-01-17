# Introduction #

`database.wdi` is a special file (much like [database.yml](http://guides.rubyonrails.org/getting_started.html#configuring-a-database) in Rails) which contains database settings for different environments.

A typical `database.wdi` may look like this:

```
deployment:
    environment development
        platform            => sqlserver2005
        host                => "(local)\sqlexpress"
        database            => oxite
        integrated-security => true
        
    environment staging
        platform            => sqlserver2005
        host                => "(local)\sqlexpress"
        database            => oxite_staging
        login               => "oxite_staging"
        password            => "0x!it3"
        
    environment production
        platform            => sqlserver2005
        host                => "(local)\sqlexpress"
        database            => oxite_production
        integrated-security => true
        allow-downgrade     => false
```

# Syntax #

The overall syntax is very similar to that of MDL files, except properties are not separated by commas, but rather by newlines and are indented as shown above.

# Environments #

You may have as many environments as you like, as long as their names are different.

## Properties ##

### Connection String ###

The following properties have special meaning in `database.wdi` for specifying connection strings:

| **Property** | **Value** |
|:-------------|:----------|
| `platform`   | Specifies the platform alias for a particular environment |
| `host`/`server` | Specifies the hostname of the database server |
| `database`   | Specifies the name of the database |
| `login`, `password` | Specify credentials used to log on to the database server |

### Guard Properties ###

The `allow-downgrade` property, which can be either `true` or `false` determines whether operations which involve downgrades (which include downgrade, rollback, redo and migration to an earlier version) are allowed for the given environment. When set to `false`, none of this commands will be executed. This is done to prevent accidental data losses in certain environments, most notably in `production`.

## Property Overrides ##

As of [r195](http://code.google.com/p/octalforty-wizardby/source/detail?r=195), Wizardby allows certain properties from the WDI file to be overridden by system environment variables.

Consider the example above. In order to set `host` to, say, just `"(local)"` without modifying the WDI file itself, just run this command in the command line:

`setx WIZARDBY_HOST "(local)"`

(you might need to restart console window) and from now on Wizardby will use `"(local"` for `host`.

This pattern applies to all available properties, including login, password, etc. Hyphens in property names are replaced with underscores:

`setx WIZARDBY_INTEGRATED_SECURITY "true"`

Casing of system environment variables names does not matter.