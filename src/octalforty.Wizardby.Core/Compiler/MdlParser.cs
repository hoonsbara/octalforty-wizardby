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
using System;
using System.Collections.Generic;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Performs syntax analysis of MDL.
    /// </summary>
    public class MdlParser : IMdlParser
    {
        #region Private Constants
        private const string MigrationKeyword = "migration";
        private static readonly Token Migration = new Token(TokenType.Keyword, MigrationKeyword, null);
        #endregion

        #region Private Types
        private delegate IAstNode BlockParser(TokenSequence sequence, IAstNode parent);

        private delegate IAstNode NodeParser(TokenSequence sequence, IAstNode parent);
        #endregion

        #region Private Fields
        private readonly IMdlScanner scanner;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MdlParser"/> class.
        /// </summary>
        /// <param name="scanner"></param>
        public MdlParser(IMdlScanner scanner)
        {
            if(scanner == null) 
                throw new ArgumentNullException("scanner");

            this.scanner = scanner;
        }

        /// <summary>
        /// Parses tokens and returns an <see cref="IAstNode"/> which represents the root node of the AST.
        /// </summary>
        /// <returns></returns>
        public IAstNode Parse()
        {
            TokenSequence tokens = scanner.Scan();
            return InternalParse(tokens, null);
        }

        private static IAstNode InternalParse(TokenSequence tokens, IAstNode parent)
        {
            if(tokens.Count == 0)
                throw CreateMdlParserException(Resources.MdlParser.UnexpectedEndOfInputWhileParsing, parent.GetType().Name);

            if(tokens.First.Equals(Migration))
                return ParseMigrationNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Deployment)))
                return ParseDeploymentNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Environment)))
                return ParseEnvironmentNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Defaults)))
                return ParseDefaultsNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.DefaultPrimaryKey)))
                return ParseDefaultPrimaryKeyNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Baseline)))
                return ParseBaselineNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Version)))
                return ParseVersionNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.TypeAliases)))
                return ParseTypeAliasesNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.TypeAlias)))
                return ParseTypeAliasNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Add)))
                return ParseAddNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Remove)))
                return ParseRemoveNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Alter)))
                return ParseAlterNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Templates)))
                return ParseTemplatesNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Refactor)))
                return ParseRefactorNode(tokens, parent);
             
            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseEnvironmentNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Location location = ParseKeyword(tokens, MdlSyntax.Environment).Location;
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        EnvironmentNode environmentNode = new EnvironmentNode(parent, name);
                        environmentNode.Location = location;

                        return environmentNode;
                    });
        }

        private static IAstNode ParseTypeAliasNode(TokenSequence tokens, IAstNode parent)
        {
            Location location = ParseKeyword(tokens, "type-alias").Location;
            return ParseTypeAliasNodeCore(tokens, parent, location);
        }

        private static IAstNode ParseTypeAliasNodeCore(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Token token = Parse(tokens, TokenType.Symbol, TokenType.StringConstant);
                        string name = token.Lexeme;
                        
                        TypeAliasNode typeAliasNode = new TypeAliasNode(parent, name);
                        typeAliasNode.Location = location ?? token.Location;

                        return typeAliasNode;
                    });
        }

        private static IAstNode ParseTypeAliasesNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Token token = ParseKeyword(tokens, MdlSyntax.TypeAliases);

                        TypeAliasesNode typeAliasesNode = new TypeAliasesNode(parent);
                        typeAliasesNode.Location = token.Location;

                        return typeAliasesNode;
                    },
                TypeAliasesNodeBlockParser);
        }

        private static IAstNode TypeAliasesNodeBlockParser(TokenSequence tokens, IAstNode parent)
        {
            //
            // If what we have next is a symbol, we have an implicit type-alias node
            if(tokens.First.Type == TokenType.Symbol)
                return ParseImplicitTypeAliasNode(tokens, parent);

            return InternalParse(tokens, parent);
        }

        private static IAstNode ParseImplicitTypeAliasNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseTypeAliasNodeCore(tokens, parent, null);
        }

        private static IAstNode ParseTemplatesNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Token token = ParseKeyword(tokens, MdlSyntax.Templates);

                        TemplatesNode templatesNode = new TemplatesNode(parent);
                        templatesNode.Location = token.Location;

                        return templatesNode;
                    },
                TemplatesNodeBlockParser);
        }

        private static IAstNode TemplatesNodeBlockParser(TokenSequence tokens, IAstNode parent)
        {
            if(tokens.First.Equals(Keyword(MdlSyntax.Table)))
                return ParseTableTemplateNode(tokens, parent);
            /*//
            // If what we have next is a symbol, we have an implicit type-alias node
            if(tokens.First.Type == TokenType.Symbol)
                return ParseImplicitTypeAliasNode(tokens, parent);*/

            return InternalParse(tokens, parent);
        }

        private static IAstNode ParseRefactorNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Token token = ParseKeyword(tokens, MdlSyntax.Refactor);
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        RefactorNode refactorNode = new RefactorNode(parent, name);
                        refactorNode.Location = token.Location;

                        return refactorNode;
                    });
        }

        private static IAstNode ParseTableTemplateNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Location location = ParseKeyword(tokens, MdlSyntax.Table).Location;
                        ParseKeyword(tokens, MdlSyntax.Template);

                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        TableTemplateNode tableTemplateNode = new TableTemplateNode(parent, name);
                        tableTemplateNode.Location = location;

                        return tableTemplateNode;
                    },
                AddTableNodeBlockParser);
        }

        private static IAstNode ParseDefaultPrimaryKeyNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Location location = ParseKeyword(tokens, MdlSyntax.DefaultPrimaryKey).Location;
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        DefaultPrimaryKeyNode defaultPrimaryKeyNode = new DefaultPrimaryKeyNode(parent, name);
                        defaultPrimaryKeyNode.Location = location;

                        return defaultPrimaryKeyNode;
                    });
        }

        private static IAstNode ParseDefaultsNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent, 
                delegate
                    {
                        Location location = ParseKeyword(tokens, MdlSyntax.Defaults).Location;

                        DefaultsNode defaultsNode = new DefaultsNode(parent);
                        defaultsNode.Location = location;

                        return defaultsNode;
                    });
        }

        private static IAstNode ParseDeploymentNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Location location = ParseKeyword(tokens, MdlSyntax.Deployment).Location;
                        
                        DeploymentNode deploymentNode = new DeploymentNode(parent);
                        deploymentNode.Location = location;

                        return deploymentNode;
                    });
        }

        private static IAstNode ParseNode(TokenSequence tokens, IAstNode parent, NodeParser nodeParser)
        {
            return ParseNode(tokens, parent, nodeParser, InternalParse);
        }

        private static IAstNode ParseNode(TokenSequence tokens, IAstNode parent, NodeParser nodeParser, BlockParser blockParser)
        {
            IAstNode astNode = nodeParser(tokens, parent);

            ParseProperties(tokens, astNode);
            ParseBlock(tokens, astNode, blockParser);

            return astNode;
        }

        private static IAstNode ParseAddTableNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            ParseKeyword(tokens, MdlSyntax.Table);
            return ParseAddTableNodeCore(tokens, parent, location);
        }

        private static IAstNode ParseAddTableNodeCore(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token token = Parse(tokens, TokenType.Symbol, TokenType.StringConstant);
                        string name = token.Lexeme;

                        AddTableNode addTableNode = new AddTableNode(parent, name);
                        addTableNode.Location = location ?? token.Location;

                        return addTableNode;
                    },
                AddTableNodeBlockParser);
            
        }

        private static IAstNode ParseImplicitAddTableNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseAddTableNodeCore(tokens, parent, null);
        }

        private static IAstNode ParseRemoveColumnNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token column = ParseKeyword(tokens, MdlSyntax.Column);
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        RemoveColumnNode removeColumnNode = new RemoveColumnNode(parent, name);
                        removeColumnNode.Location = location ?? column.Location;

                        return removeColumnNode;
                    }, 
                AddTableNodeBlockParser);
        }

        private static IAstNode ParseRemoveTableNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token table = ParseKeyword(tokens, MdlSyntax.Table);
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        RemoveTableNode removeTableNode = new RemoveTableNode(parent, name);
                        removeTableNode.Location = location ?? table.Location;

                        return removeTableNode;
                    }, 
                AddTableNodeBlockParser);
        }

        private static IAstNode ParseAlterTableNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        ParseKeyword(tokens, "table");
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        AlterTableNode alterTableNode = new AlterTableNode(parent, name);

                        return alterTableNode;
                    });
        }

        private static IAstNode ParseAlterColumnNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        ParseKeyword(tokens, "column");
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        AlterColumnNode alterColumnNode = new AlterColumnNode(parent, name);

                        return alterColumnNode;
                    });
        }

        private static IAstNode ParseRemoveIndexNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token index = ParseKeyword(tokens, MdlSyntax.Index);
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        RemoveIndexNode removeIndexNode = new RemoveIndexNode(parent, name);
                        removeIndexNode.Location = location ?? index.Location;

                        return removeIndexNode;
                    }, 
                AddTableNodeBlockParser);
        }

        private static IAstNode ParseRemoveReferenceNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token reference = ParseKeyword(tokens, MdlSyntax.Reference);
                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        RemoveReferenceNode removeReferenceNode = new RemoveReferenceNode(parent, name);
                        removeReferenceNode.Location = location ?? reference.Location;

                        return removeReferenceNode;
                    }, 
                AddTableNodeBlockParser);
        }

        private static IAstNode AddTableNodeBlockParser(TokenSequence tokens, IAstNode parent)
        {
            if(tokens.First.Type == TokenType.Symbol)
                return ParseImplicitAddColumnNode(tokens, parent);

            if(tokens.First.Equals(Keyword(MdlSyntax.Add)))
                return ParseAddNode(tokens, parent);

            if(tokens.First.Equals(Keyword(MdlSyntax.Index)))
                return ParseImplicitAddIndexNode(tokens, parent, null);

            if(tokens.First.Equals(Keyword(MdlSyntax.Reference)))
                return ParseImplicitAddReferenceNode(tokens, parent, null);

            if(tokens.First.Equals(Keyword(MdlSyntax.Constraint)))
                return ParseAddConstraintNode(tokens, parent, null);

            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseImplicitAddIndexNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            Token index = ParseKeyword(tokens, MdlSyntax.Index);
            return ParseAddIndexNodeCore(tokens, parent, location ?? index.Location);
        }

        private static IAstNode ParseAddIndexNodeCore(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        //
                        // If parent is an IAddColumnNode the index can be anonymous.
                        // To distinguish an anonymous index from the named one we analyze the next
                        // token. If it's a symbol, the index is considered to be anonymous. If it's
                        // a string constant, the index is considered to be named.
                        Location nameLocation;
                        string name = ParsePossiblyAnonymousNodeNameAsChildOf<IAddColumnNode>(tokens, parent, out nameLocation);
                        
                        AddIndexNode addIndexNode = new AddIndexNode(parent, name);
                        addIndexNode.Location = location ?? nameLocation;

                        return addIndexNode;
                    });
        }

        private static IAstNode ParseAddConstraintNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                {
                    ParseKeyword(tokens, MdlSyntax.Constraint);

                    //
                    // If parent is an IAddColumnNode the constraint can be anonymous.
                    Location nameLocation;
                    string name = ParsePossiblyAnonymousNodeNameAsChildOf<IAddColumnNode>(tokens, parent, out nameLocation);

                    AddConstraintNode addConstraintNode = new AddConstraintNode(parent, name);
                    addConstraintNode.Location = location ?? nameLocation;

                    return addConstraintNode;
                });
        }

        private static IAstNode ParseRemoveConstraintNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                {
                    Token index = ParseKeyword(tokens, MdlSyntax.Constraint);
                    string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                    RemoveConstraintNode removeConstraintNode = new RemoveConstraintNode(parent, name);
                    removeConstraintNode.Location = location ?? index.Location;

                    return removeConstraintNode;
                });
        }

        private static IAstNode ParseAddNode(TokenSequence tokens, IAstNode parent)
        {
            Token add = ParseKeyword(tokens, MdlSyntax.Add);

            if(tokens.First.Equals(Keyword(MdlSyntax.Table)))
                return ParseAddTableNode(tokens, parent, add.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Column)))
                return ParseAddColumnNode(tokens, parent, add.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Index)))
                return ParseImplicitAddIndexNode(tokens, parent, add.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Reference)))
                return ParseImplicitAddReferenceNode(tokens, parent, add.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Constraint)))
                return ParseAddConstraintNode(tokens, parent, add.Location);

            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseRemoveNode(TokenSequence tokens, IAstNode parent)
        {
            Token remove = ParseKeyword(tokens, MdlSyntax.Remove);

            if(tokens.First.Equals(Keyword(MdlSyntax.Table)))
                return ParseRemoveTableNode(tokens, parent, remove.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Column)))
                return ParseRemoveColumnNode(tokens, parent, remove.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Index)))
                return ParseRemoveIndexNode(tokens, parent, remove.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Reference)))
                return ParseRemoveReferenceNode(tokens, parent, remove.Location);

            if(tokens.First.Equals(Keyword(MdlSyntax.Constraint)))
                return ParseRemoveConstraintNode(tokens, parent, remove.Location);

            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseAlterNode(TokenSequence tokens, IAstNode parent)
        {
            ParseKeyword(tokens, "alter");

            if(tokens.First.Equals(Keyword("table")))
                return ParseAlterTableNode(tokens, parent);
            if(tokens.First.Equals(Keyword("column")))
                return ParseAlterColumnNode(tokens, parent);
            /*if(tokens.First.Equals(Keyword("index")))
                return ParseRemoveIndexNode(tokens, parent);
            if(tokens.First.Equals(Keyword("reference")))
                return ParseRemoveReferenceNode(tokens, parent);*/

            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseAddColumnNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            ParseKeyword(tokens, MdlSyntax.Column);
            return ParseAddColumnNodeCore(tokens, parent, location);
        }

        private static IAstNode ParseImplicitAddColumnNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseAddColumnNodeCore(tokens, parent, null);
        }

        private static IAstNode ParseAddColumnNodeCore(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token token = Parse(tokens, TokenType.Symbol, TokenType.StringConstant);
                        string name = token.Lexeme;
                        
                        AddColumnNode addColumnNode = new AddColumnNode(parent, name);
                        addColumnNode.Location = location ?? token.Location;

                        return addColumnNode;
                    },
                AddColumnNodeBlockParser);
        }

        private static IAstNode AddColumnNodeBlockParser(TokenSequence tokens, IAstNode parent)
        {
            if(tokens.First.Equals(Keyword(MdlSyntax.Index)))
                return ParseImplicitAddIndexNode(tokens, parent, null);
            if(tokens.First.Equals(Keyword(MdlSyntax.Reference)))
                return ParseImplicitAddReferenceNode(tokens, parent, null);
            if(tokens.First.Equals(Keyword(MdlSyntax.Add)))
                return ParseAddNode(tokens, parent);
            if(tokens.First.Equals(Keyword(MdlSyntax.Constraint)))
                return ParseAddConstraintNode(tokens, parent, null);

            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseImplicitAddReferenceNode(TokenSequence tokens, IAstNode parent, Location location)
        {
            Token reference = ParseKeyword(tokens, MdlSyntax.Reference);
            return ParseAddReferenceNodeCore(tokens, parent, location ?? reference.Location);
        }

        private static IAstNode ParseAddReferenceNodeCore(TokenSequence tokens, IAstNode parent, Location location)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        //
                        // Refer to ParseAddIndexNodeCore on how to distinguish anonymous
                        // nodes from named ones. Add Reference can be anonymous as a child
                        // of IAddColumnNode
                        Location nameLocation;
                        string name = ParsePossiblyAnonymousNodeNameAsChildOf<IAddColumnNode>(tokens, parent, out nameLocation);

                        AddReferenceNode addReferenceNode = new AddReferenceNode(parent, name);
                        addReferenceNode.Location = location ?? nameLocation;

                        return addReferenceNode;
                    });
        }

        private static string ParsePossiblyAnonymousNodeNameAsChildOf<T>(TokenSequence tokens, IAstNode parent, out Location location)
            where T : IAstNode
        {
            if(parent is T)
                return ParsePossiblyAnonymousNodeName(tokens, out location);

            Token token = Parse(tokens, TokenType.StringConstant, TokenType.Symbol);
            location = token.Location;

            return token.Lexeme;
        }

        private static string ParsePossiblyAnonymousNodeName(TokenSequence tokens, out Location location)
        {
            if(tokens.First.Type == TokenType.StringConstant)
            {
                Token token = Parse(tokens, TokenType.StringConstant);

                location = token.Location;
                return token.Lexeme;
            } // if

            location = null;
            return String.Empty;
        }

        private static IAstNode ParseVersionNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token version = ParseKeyword(tokens, MdlSyntax.Version);

                        if(!NextTokenIsOneOf(tokens, TokenType.IntegerConstant))
                            throw CreateMdlParserException(Resources.MdlParser.ExpectedVersionNumberForVersionAt,
                                TranslateLocation(version.Location));

                        long number = Int64.Parse(Parse(tokens, TokenType.IntegerConstant).Lexeme);

                        VersionNode versionNode = new VersionNode(parent, number);
                        versionNode.Location = version.Location;

                        return versionNode;
                    },
                VersionNodeBlockParser);
        }

        private static IAstNode VersionNodeBlockParser(TokenSequence tokens, IAstNode parent)
        {
            //
            // If first token is a symbol, then we're parsing implicit add table node
            if(tokens.First.Type == TokenType.Symbol)
                return ParseImplicitAddTableNode(tokens, parent);
            
            return InternalParse(tokens, parent);
        }

        private static IAstNode ParseBaselineNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token token = ParseKeyword(tokens, MdlSyntax.Baseline);
                        
                        BaselineNode baselineNode = new BaselineNode(parent);
                        baselineNode.Location = token.Location;

                        return baselineNode;
                    },
                BaselineNodeBlockParser);
        }

        private static IAstNode BaselineNodeBlockParser(TokenSequence tokens, IAstNode parent)
        {
            if(tokens.First.Equals(Keyword(MdlSyntax.Add)))
                return ParseAddNode(tokens, parent);

            if(tokens.First.Equals(Keyword(MdlSyntax.Table)))
                return ParseAddTableNode(tokens, parent, tokens.First.Location);

            throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt,
                tokens.First.Lexeme, tokens.First.Type, TranslateLocation(tokens.First.Location));
        }

        private static IAstNode ParseMigrationNode(TokenSequence tokens, IAstNode parent)
        {
            return ParseNode(tokens, parent,
                delegate
                    {
                        Token migration = ParseKeyword(tokens, MigrationKeyword);

                        if(!NextTokenIsOneOf(tokens, TokenType.Symbol, TokenType.StringConstant))
                            throw CreateMdlParserException(Resources.MdlParser.ExpectedMigrationNameForMigrationAt, 
                                TranslateLocation(migration.Location));

                        string name = Parse(tokens, TokenType.Symbol, TokenType.StringConstant).Lexeme;

                        //
                        // We must ensure that what follows is either a symbol, a colon or an end of statement - otherwise
                        // what we've just read is not a migration name
                        if(!NextTokenIsOneOf(tokens, TokenType.Symbol, TokenType.Colon, TokenType.EndStatement))
                            throw CreateMdlParserException(Resources.MdlParser.ExpectedMigrationNameForMigrationAt,
                                TranslateLocation(migration.Location));

                        MigrationNode migrationNode = new MigrationNode(parent, name);
                        migrationNode.Location = migration.Location;

                        return migrationNode;
                    });
        }

        private static string TranslateLocation(Location location)
        {
            return location == null ?
                "()" :
                string.Format("{0}:{1}", location.Line + 1, location.Column + 1);
        }

        private static MdlParserException CreateMdlParserException(string formatString, params object[] args)
        {
            return new MdlParserException(string.Format(formatString, args));
        }

        private static void ParseBlock(TokenSequence tokens, IAstNode parent)
        {
            ParseBlock(tokens, parent, InternalParse);
        }

        private static void ParseBlock(TokenSequence tokens, IAstNode parent, BlockParser blockParser)
        {
            //
            // If what we have next is colon, parse the block. Otherwise,
            // parse and EndStatement and return
            if(tokens.First.Type != TokenType.Colon)
            {
                Parse(tokens, TokenType.EndStatement);
                return;
            } // if

            Parse(tokens, TokenType.Colon);
            Parse(tokens, TokenType.EndStatement);
            Parse(tokens, TokenType.BeginBlock);

            while(tokens.First.Type != TokenType.EndBlock)
            {
                parent.ChildNodes.Add(blockParser(tokens, parent));

                //
                // If we have a BeginBlock here, then report incorrect layout
                if(tokens.First.Type == TokenType.BeginBlock)
                    throw CreateMdlParserException(Resources.MdlParser.IncorrectLayoutAt, TranslateLocation(tokens.First.Location));
            } // while

            Parse(tokens, TokenType.EndBlock);
        }

        private static void ParseProperties(TokenSequence tokens, IAstNode astNode)
        {
            //
            // If we encounter an EndStatement, we might potentially be parsing a property block
            if(tokens.First.Type == TokenType.EndStatement)
            {
                //
                // Cache that in case we're mistaking
                Token endStatement = tokens.RemoveFirst();
                if(tokens.Count == 0 || tokens.First.Type != TokenType.BeginBlock)
                {
                    tokens.InsertFirst(endStatement);
                    return;
                } // if

                ParsePropertyBlock(tokens, astNode);
                return;
            } // if

            while(!tokens.Empty && tokens.First.Type != TokenType.Colon && tokens.First.Type != TokenType.EndStatement)
            {
                IAstNodeProperty property = ParsePropertyCore(tokens);
                astNode.Properties.AddProperty(property);

                if(tokens.Count == 0 || tokens.First.Type != TokenType.Comma)
                    break;
                
                tokens.RemoveFirst();
            } // while
        }

        private static void ParsePropertyBlock(TokenSequence tokens, IAstNode astNode)
        {
            //
            // We're surely parsing property block. Go ahead
            Parse(tokens, TokenType.BeginBlock);
            while(!tokens.Empty && tokens.First.Type != TokenType.EndBlock)
            {
                IAstNodeProperty property = ParsePropertyCore(tokens);
                astNode.Properties.AddProperty(property);

                if(!tokens.Empty && (tokens.First.Type == TokenType.EndStatement ||tokens.First.Type == TokenType.Comma))
                    tokens.RemoveFirst();
            } // while

            tokens.RemoveFirst();

            //
            // Add artificial EndStatement to please the rest of the parser
            tokens.InsertFirst(new Token(TokenType.EndStatement, null));
        }

        private static IAstNodeProperty ParsePropertyCore(TokenSequence tokens)
        {
            Token propertyNameToken = Parse(tokens, TokenType.Symbol, TokenType.Keyword);
            string propertyName = propertyNameToken.Lexeme;
            
            Parse(tokens, TokenType.PropertyAssignment);

            PropertyValue propertyValue = ParsePropertyValue(tokens);
            return new AstNodeProperty(propertyName, GetAstNodePropertyValue(propertyValue), propertyNameToken.Location);
        }

        private static IAstNodePropertyValue GetAstNodePropertyValue(PropertyValue propertyValue)
        {
            switch(propertyValue.Type)
            {
                case PropertyValueType.List:
                    List<IAstNodePropertyValue> values = new List<IAstNodePropertyValue>();
                    foreach(PropertyValue innerValue in ((PropertyValue[])propertyValue.Value))
                        values.Add(GetAstNodePropertyValue(innerValue));

                    return new ListAstNodePropertyValue(values.ToArray());
                case PropertyValueType.String:
                    return new StringAstNodePropertyValue((string)propertyValue.Value);
                case PropertyValueType.Symbol:
                    return new SymbolAstNodePropertyValue((string)propertyValue.Value);
                case PropertyValueType.Integer:
                    return new IntegerAstNodePropertyValue((int)propertyValue.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static PropertyValue ParsePropertyValue(TokenSequence tokens)
        {
            //
            // If what we have next is a '[' symbol, then we're parsing a list
            return tokens.First.Type == TokenType.LeftSquareBracket ?
                ParseListPropertyValue(tokens) :
                ParseSimplePropertyValue(tokens);
        }

        private static PropertyValue ParseListPropertyValue(TokenSequence tokens)
        {
            Parse(tokens, TokenType.LeftSquareBracket);

            List<PropertyValue> propertyValues = new List<PropertyValue>();
            while(tokens.First.Type != TokenType.RightSquareBracket)
            {
                propertyValues.Add(ParsePropertyValue(tokens));

                if (tokens.First.Type == TokenType.Comma)
                    Parse(tokens, TokenType.Comma);
            } // while

            Parse(tokens, TokenType.RightSquareBracket);

            return new PropertyValue(PropertyValueType.List, propertyValues.ToArray());
        }

        private static PropertyValue ParseSimplePropertyValue(TokenSequence tokens)
        {
            Token propertyValue = Parse(tokens, TokenType.IntegerConstant, TokenType.StringConstant, TokenType.Symbol);
            return GetPropertyValue(propertyValue);
        }
        
        private static PropertyValue GetPropertyValue(Token propertyValue)
        {
            switch (propertyValue.Type)
            {
                case TokenType.Symbol:
                    return new PropertyValue(PropertyValueType.Symbol, propertyValue.Lexeme);
                case TokenType.StringConstant:
                    return new PropertyValue(PropertyValueType.String, propertyValue.Lexeme);
                case TokenType.IntegerConstant:
                    return new PropertyValue(PropertyValueType.Integer, Convert.ToInt32(propertyValue.Lexeme));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Token Keyword(string lexeme)
        {
            return new Token(TokenType.Keyword, lexeme, null);
        }

        private static Token ParseKeyword(TokenSequence tokens, string keyword)
        {
            if(!tokens.First.Equals(Keyword(keyword)))
                throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type, 
                    TranslateLocation(tokens.First.Location));

            return tokens.RemoveFirst();
        }

        private static Token Parse(TokenSequence tokens, params TokenType[] allowedTypes)
        {
            if(Array.IndexOf(allowedTypes, tokens.First.Type) == -1)
                throw CreateMdlParserException(Resources.MdlParser.UnexpectedTokenAt, tokens.First.Lexeme, tokens.First.Type,
                    TranslateLocation(tokens.First.Location));

            return tokens.RemoveFirst();
        }

        private static bool NextTokenIsOneOf(TokenSequence tokens, params TokenType[] allowedTypes)
        {
            return !tokens.Empty && Array.IndexOf(allowedTypes, tokens.First.Type) != -1;
        }
    }
}
