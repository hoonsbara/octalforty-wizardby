using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using octalforty.Wizardby.Db.SqlServer2005;

namespace octalforty.Wizardby.Db.SqlServer2008
{
    public class SqlServer2008TypeMapper : SqlServer2005TypeMapper
    {
        private const string Time = "time";

        public SqlServer2008TypeMapper()
        {
            RegisterTypeMapping(DbType.Time, Time);
        }

        public override DbType? MapToDbType(string nativeType, int? length)
        {
            if(nativeType.ToLowerInvariant() == Time) return DbType.Time;

            return base.MapToDbType(nativeType, length);
        }
    }
}
