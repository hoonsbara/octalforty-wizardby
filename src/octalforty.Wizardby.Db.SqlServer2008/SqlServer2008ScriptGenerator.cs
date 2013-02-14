using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlServer2008
{
    public class SqlServer2008ScriptGenerator : SqlServer2000ScriptGenerator
    {
        public SqlServer2008ScriptGenerator(IDbStatementBatchWriter statementBatchWriter) : 
            base(statementBatchWriter)
        {
        }

        public override void Visit(IAddIndexNode addIndexNode)
        {
            if(string.IsNullOrWhiteSpace(addIndexNode.Where))
                base.Visit(addIndexNode);
            else
            {
                var createIndexBuilder = new StringBuilder("create ");

                if(addIndexNode.Unique ?? false)
                    createIndexBuilder.Append("unique ");

                if(Platform.Capabilities.IsSupported(DbPlatformCapabilities.SupportsClusteredIndexes))
                    if(addIndexNode.Clustered ?? false)
                        createIndexBuilder.Append("clustered ");
                    else
                        createIndexBuilder.Append("nonclustered ");

                createIndexBuilder.AppendFormat("index {0} on {1} ({2}) where {3};",
                    Platform.Dialect.EscapeIdentifier(addIndexNode.Name),
                    Platform.Dialect.EscapeIdentifier(addIndexNode.Table),
                    Join(", ", GetIndexColumnsDefinitions(addIndexNode.Columns)),
                    addIndexNode.Where);

                TextWriter.WriteLine(createIndexBuilder.ToString());
            }
        }

    }
}
