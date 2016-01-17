# Introduction #

Add your content here.


# Proposed Syntax #

```
migration "hglab": version = 1 {
    defaults {
        default-pk ID type: Int64, pk: true, nullable: false
    }

    type-aliases {
        S: type = String, nullable = false, length = 200
        NS: type = String, nullable = true, length = 200        
    }

    version 20120604195202 {
        IdentityInfo: pk = false {
            Hi "Int64!"
        }
            
        sql {
            platform "sqlserver2008" {
                upgrade /*: script = */ "initialize_identityinfo"
                downgrade: ref = "delete_idnetityinfo"
            }
        }
        
        User {
            FullName "string!(200)"
            EmailAddress SL: unique = true
            PasswordHash NSL
        }
                
        Project {
            Name SL
            Slug SL: unique = true
            Description: type NSXL
            ManagerUserID: references User, nullable true {
                index
            }
        }

        Milestone {
            ProjectID references: Project
            Name type: SL
            Slug type: SL, unique: true
            Description type: NSXL
            DueOn type: "datetime?"
        }

        Repository {
            ProjectID references: Project {
                index
            }
            Name type: SL {
                index
            }

            index columns: [ProjectID, Name], unique: true
        }        
    }

    version 20120613183158 {
        Setting {
            Module type: SL
            Property type: SL
            Value type: NSXL

            index columns: [Module, Property], unique: true
        }
    }

    version 20120614173519 {
        alter table Repository {
            add column Status type: "int32!"
            add column Description type: NSXL
        }
    }

    version 20120713065451 {
        alter table Repository {
            add column StatsCommits type: "int32?"
            add column StatsLastActivityAt type: "datetime?"
            add column StatsCommitActivity type: "string?"
        }

        sql {
            shared {
                upgrade script-ref: "update_repository_stats"
            }
            sqlite {
                upgrade script-ref: "update_repository_stats_sqlite"
            }
        }
        execute native-sql upgrade-resource: "update_repository_stats"

        alter table Repository {
            alter column StatsCommits nullable: false
            alter column StatsCommitActivity nullable: false
        }
    }

    version 20120720092613 {
        alter table User {
            add column Login type: NS, unique: true
        }

        execute native-sql upgrade-resource: "update_user_login"

        alter table User {
            alter column Login nullable: false
        }
    }

    version 20120720164056 {
        Group {
            Name type: SL
            Slug type: SL, unique: true
        }

        UserGroupJunction pk: false {
            UserID references: User, index: true
            GroupID references: Group, index: true

            index columns: [UserID, GroupID], unique: true
        }
    }

    version 20120723161153 {
        alter table Repository {
            add column SecurityMode type: "int32?"
        }

        execute native-sql upgrade-resource: "update_repository_securitymode"

        alter table Repository {
            alter column SecurityMode nullable: false
        }
    }
}
```