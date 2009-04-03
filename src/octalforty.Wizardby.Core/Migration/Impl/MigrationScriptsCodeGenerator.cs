#region The MIT License
// The MIT License
// 
// Copyright (c) 2009 octalforty studios
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using System.Collections.Generic;
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class MigrationScriptsCodeGenerator : CodeGeneratorBase
    {
        private readonly IDbPlatform dbPlatform;
        private readonly MigrationMode migrationMode;
        private readonly MigrationScriptCollection migrationScripts = new MigrationScriptCollection();

        public MigrationScriptCollection MigrationScripts
        {
            get { return migrationScripts; }
        }

        public MigrationScriptsCodeGenerator(IDbPlatform dbPlatform, MigrationMode migrationMode)
        {
            this.dbPlatform = dbPlatform;
            this.migrationMode = migrationMode;
        }

        public override void Visit(IMigrationNode migrationNode)
        {
            StringBuilder sb = new StringBuilder();

            /*IAstNode m = null;
            if(migrationMode == MigrationMode.Upgrade)
                m = GetFirst<IUpgradeNode>(migrationNode.ChildNodes);
            else if (migrationMode == MigrationMode.Downgrade)
                m = GetFirst<IDowngradeNode>(migrationNode.ChildNodes);*/

            foreach(IVersionNode versionNode in Filter<IVersionNode>(migrationNode.ChildNodes))
            {
                List<string> ddlScripts = new List<string>();

                foreach(IAstNode upgradeNode in GetNode(versionNode, migrationMode).ChildNodes)
                {
                    using (TextWriter tw = new StringWriter(sb))
                    {
                        IDbScriptGenerator scriptGenerator = dbPlatform.Dialect.CreateScriptGenerator(tw);
                        upgradeNode.Accept(scriptGenerator);
                    } // using

                    ddlScripts.Add(sb.ToString());
                    sb.Length = 0;
                }

                migrationScripts.Add(new MigrationScript(versionNode.Number, ddlScripts.ToArray()));
            } // foreach
        }

        private IAstNode GetNode(IVersionNode versionNode, MigrationMode migrationMode)
        {
            return migrationMode == MigrationMode.Upgrade ?
                (IAstNode)GetFirst<IUpgradeNode>(versionNode.ChildNodes) :
                (IAstNode)GetFirst<IDowngradeNode>(versionNode.ChildNodes);
        }
    }
}
