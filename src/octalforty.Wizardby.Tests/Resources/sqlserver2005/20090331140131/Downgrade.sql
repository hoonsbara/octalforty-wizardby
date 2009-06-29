if exists(select * from sys.objects where object_id = object_id(N'dbo.Sample') and type in (N'P', N'PC'))
    drop procedure dbo.Sample