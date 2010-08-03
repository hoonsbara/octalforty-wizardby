using System;
using System.IO;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Console.Commands
{
    [MigrationCommand(MigrationCommand.Update)]
    public class UpdateMigrationCommand : MigrationCommandBase
    {
        protected override void InternalExecute(MigrationParameters parameters)
        {
            IAstNode astNode;
            using(var streamReader = new StreamReader(parameters.MdlFileName))
                astNode = new MdlCompiler(null, new Core.Compiler.Environment()).Compile(streamReader,
                    MdlCompilationOptions.InferTypes | MdlCompilationOptions.ResolvePrimaryKeys | 
                    MdlCompilationOptions.ResolvePropertyValues | MdlCompilationOptions.ResolveTypeAliases);

            System.Console.WriteLine();

            var migrationNode = (IMigrationNode)astNode;
            if(migrationNode.Revision == 2)
            {
                using(new ConsoleStylingScope(ConsoleColor.Green))
                    System.Console.WriteLine(Resources.MigrationDefinitionIsUpToDate);

                return;
            } // if

            using(new ConsoleStylingScope(ConsoleColor.Yellow))
                System.Console.WriteLine(Resources.UpdatingMigrationDefinition, migrationNode.Revision, 2);

            string migrationDefinition;
            using(var streamReader = new StreamReader(parameters.MdlFileName))
                migrationDefinition = streamReader.ReadToEnd();

            migrationDefinition = migrationDefinition.Replace("revision => 1", "revision => 2");

            var updateMigration =
                @"migration ""__wizardby"" revision => 2:
    version 1:
        alter table SchemaInfo:
            add column Module type => String, length => 200, nullable => true";

            using(var streamWriter = new StreamWriter(parameters.MdlFileName, false))
                streamWriter.WriteLine(migrationDefinition);

        }
    }
}