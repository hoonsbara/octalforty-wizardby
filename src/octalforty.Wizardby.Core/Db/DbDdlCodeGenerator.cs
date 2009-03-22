using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db.Impl;

namespace octalforty.Wizardby.Core.Db
{
    public class DbDdlCodeGenerator : AstVisitorBase, Compiler.ICodeGenerator
    {
        #region Private Fields
        private readonly IDbPlatform dbPlatform;
        private readonly IndentedTextWriter textWriter;
        private readonly int[] versions;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DbDdlCodeGenerator"/> class.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="textWriter"></param>
        public DbDdlCodeGenerator(IDbPlatform dbPlatform, TextWriter textWriter, params int[] versions)
        {
            this.dbPlatform = dbPlatform;
            this.versions = versions;
            this.textWriter = new IndentedTextWriter(textWriter);
        }

        #region AstVisitorBase Members
        public override void Visit(IMigrationNode migrationNode)
        {
            Visit(migrationNode.ChildNodes);
        }

        public override void Visit(IBaselineNode baselineNode)
        {
        }

        public override void Visit(IVersionNode versionNode)
        {
            if(Array.IndexOf(versions, versionNode.Number) != -1)
                Visit(versionNode.ChildNodes);
        }

        public override void Visit(ICreateTableNode createTableNode)
        {
            textWriter.WriteLine(dbPlatform.Dialect.StartCreateTable(dbPlatform.Dialect.EscapeIdentifier(createTableNode.Name)));
                        
            using(new IndentScope(textWriter))
            {
                bool firstColumn = true;
                foreach(IAddColumnNode addColumnNode in Filter<IAddColumnNode>(createTableNode.ChildNodes))
                {
                    if(!firstColumn) textWriter.WriteLine(",");
                    else firstColumn = false;

                    Visit(addColumnNode);
                } // foreach
            } // using

            textWriter.WriteLine(dbPlatform.Dialect.EndCreateTable());
        }

        public override void Visit(IAddColumnNode addColumnNode)
        {
            try
            {
                /*textWriter.Write(dbPlatform.Dialect.BeginAddColumnToNewTable(dbPlatform.Dialect.EscapeIdentifier(addColumnNode.Name),
                    dbPlatform.TypeMapper.MapToNativeType(addColumnNode.Type.Value, addColumnNode.Length),
                    addColumnNode.Nullable.Value));*/
            } // try

            catch
            {
                Trace.WriteLine(string.Format("{0} in {1}", addColumnNode.Name, ((ICreateTableNode)addColumnNode.Parent).Name));
            }
        }

        public override void Visit(IAddIndexNode addIndexNode)
        {
            base.Visit(addIndexNode);
        }

        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            base.Visit(addReferenceNode);
        }

        public override void Visit(IDeploymentNode deploymentNode)
        {
            base.Visit(deploymentNode);
        }

        public override void Visit(IDatabaseNode databaseNode)
        {
            base.Visit(databaseNode);
        }

        public override void Visit(IDefaultsNode defaultsNode)
        {
            base.Visit(defaultsNode);
        }

        public override void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode)
        {
            base.Visit(defaultPrimaryKeyNode);
        }

        protected override void Visit(IEnumerable<IAstNode> astNodes)
        {
            base.Visit(astNodes);
        }

        protected override IEnumerable<IAstNode> Filter<T>(IEnumerable<IAstNode> astNodes)
        {
            return base.Filter<T>(astNodes);
        }

        protected override T GetFirst<T>(IEnumerable<IAstNode> astNodes)
        {
            return base.GetFirst<T>(astNodes);
        }
        #endregion

    }
}
