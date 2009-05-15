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
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Core.Util;
using octalforty.Wizardby.Db.SqlServer2000;
using octalforty.Wizardby.Tests.Core.Compiler;

namespace octalforty.Wizardby.Tests.Db.SqlServer2000
{
    [TestFixture()]
    public class SqlServer2000ScriptGeneratorTestFixture
    {
        [Test()]
        public void GenerateScript()
        {
            IAstNode astNode;
            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();
            } // using

            IDbPlatform platform = new SqlServer2000Platform();
            Environment environment = new Environment();
            IMdlCompiler compiler = new MdlCompiler(new NullCodeGenerator(), environment);

            compiler.RemoveCompilerStage<DowngradeGenerationStage>();
            compiler.RemoveCompilerStage<UpgradeGenerationStage>();
            compiler.AddCompilerStageAfter<AstFlattenerCompilerStage>(new DbNamingCompilerStage(platform.NamingStrategy));
            compiler.Compile(astNode, MdlCompilationOptions.All);

            StringBuilder stringBuilder = new StringBuilder();
            
            IDbScriptGenerator scriptGenerator = platform.Dialect.CreateScriptGenerator(new StringWriter(stringBuilder));
            scriptGenerator.SetEnvironment(environment);

            foreach(IVersionNode versionNode in Algorithms.Filter<IAstNode, IVersionNode>(astNode.ChildNodes))
                versionNode.Accept(scriptGenerator);

            Assert.AreEqual(@"create table [SchemaInfo] (
[Version] bigint not null,
);
create table [Author] (
[ID] int not null identity primary key,
[FirstName] nvarchar(200) not null,
[LastName] nvarchar(200) not null,
[EmailAddress] nvarchar(200) not null,
[Login] nvarchar(200) not null,
[Password] varbinary(64) null,
);
create table [Tag] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
);
create table [Blog] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
[Description] nvarchar(max) not null,
);
create table [BlogPost] (
[ID] int not null identity primary key,
[Title] nvarchar(200) not null,
[Slug] nvarchar(200) not null,
[BlogID] int not null,
[AuthorID] int not null,
);
create table [BlogPostTagJunction] (
[BlogPostID] int not null,
[TagID] int not null,
);
create unique nonclustered index [UQ_Version] on [SchemaInfo] ([Version]);
create unique nonclustered index [IX_EmailAddress] on [Author] ([EmailAddress]);
create unique nonclustered index [IX_Login] on [Author] ([Login]);
alter table [BlogPost] add constraint [FK1] foreign key ([BlogID]) references [Blog] ([ID]);
alter table [BlogPost] add constraint [FK2] foreign key ([AuthorID]) references [Author] ([ID]);
alter table [BlogPostTagJunction] add constraint [FK3] foreign key ([BlogPostID]) references [BlogPost] ([ID]);
alter table [BlogPostTagJunction] add constraint [FK4] foreign key ([TagID]) references [Tag] ([ID]);
create table [BlogPostComment] (
[ID] int not null identity primary key,
[BlogPostID] int not null,
[AuthorEmailAddress] nvarchar(200) not null,
[Content] nvarchar(max) not null,
);
alter table [BlogPostComment] add constraint [FK5] foreign key ([BlogPostID]) references [BlogPost] ([ID]);
create table [Media] (
[ID] int not null identity primary key,
[TypeID] int ,
[Name] nvarchar(200) not null,
[MimeType] nvarchar(200) not null default ('text/xml'),
[Length] int ,
[BlogPostID] int null,
[BlogPostCommentID] int null,
);
create table [User] (
[ID] int not null identity primary key,
[Login] nvarchar(200) not null,
[Password] varbinary(64) not null,
);
alter table [Media] add constraint [FK10] foreign key ([BlogPostID]) references [BlogPost] ([ID]);
alter table [Media] alter column [MimeType] set default 'text/xml';
alter table [Media] add constraint [FK11] foreign key ([BlogPostCommentID]) references [BlogPostComment] ([ID]);
create unique nonclustered index [IX_Login] on [User] ([ID], [Login] desc);
create table [Forum] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
[ModeratorUserID] int not null,
);
alter table [Forum] add constraint [FK_FOO] foreign key ([ModeratorUserID]) references [User] ([ID]);
drop index [IX_Login] on [User];
create unique nonclustered index [IX_Login] on [User] ([ID], [Login] desc);
create table [BlogAuthorJunction] (
[BlogID] int not null,
[AuthorID] int not null,
);
alter table [BlogAuthorJunction] add constraint [FK12] foreign key ([BlogID]) references [Blog] ([ID]);
alter table [BlogAuthorJunction] add constraint [FK13] foreign key ([AuthorID]) references [Author] ([ID]);
alter table [Forum] add [Slug] nvarchar(200) not null;
alter table [Forum] drop column [Slug];
alter table [Forum] add [Slug] nvarchar(200) not null;
alter table [Forum] alter column [Slug] nvarchar(200) null;
alter table [Forum] drop constraint [FK_FOO];
drop table [Forum];
alter table [BlogPostTagJunction] drop constraint [FK4];
drop table [Tag];
", stringBuilder.ToString());
        }

        [Test()]
        public void GenerateScript2()
        {
            IAstNode astNode;
            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.BlogWithSchemas.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();
            } // using

            IDbPlatform platform = new SqlServer2000Platform();
            IMdlCompiler compiler = new MdlCompiler(new NullCodeGenerator(), new Environment());

            compiler.RemoveCompilerStage<DowngradeGenerationStage>();
            compiler.RemoveCompilerStage<UpgradeGenerationStage>();
            compiler.AddCompilerStageAfter<AstFlattenerCompilerStage>(new DbNamingCompilerStage(platform.NamingStrategy));
            compiler.Compile(astNode, MdlCompilationOptions.All);

            StringBuilder stringBuilder = new StringBuilder();


            IDbScriptGenerator scriptGenerator = platform.Dialect.CreateScriptGenerator(new StringWriter(stringBuilder));

            foreach (IVersionNode versionNode in Algorithms.Filter<IAstNode, IVersionNode>(astNode.ChildNodes))
                versionNode.Accept(scriptGenerator);

            Assert.AreEqual(@"create table [SchemaInfo] (
[Version] bigint not null,
);
create table [a].[Author] (
[ID] int not null identity primary key,
[FirstName] nvarchar(200) not null,
[LastName] nvarchar(200) not null,
[EmailAddress] nvarchar(200) not null,
[Login] nvarchar(200) not null,
[Password] varbinary(64) null,
);
create table [t].[Tag] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
);
create table [b].[Blog] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
[Description] nvarchar(max) not null,
);
create table [bp].[BlogPost] (
[ID] int not null identity primary key,
[Title] nvarchar(200) not null,
[Slug] nvarchar(200) not null,
[BlogID] int not null,
[AuthorID] int not null,
);
create table [bptj].[BlogPostTagJunction] (
[BlogPostID] int not null,
[TagID] int not null,
);
create unique nonclustered index [UQ_Version] on [SchemaInfo] ([Version]);
create unique nonclustered index [IX_EmailAddress] on [a].[Author] ([EmailAddress]);
create unique nonclustered index [IX_Login] on [a].[Author] ([Login]);
alter table [bp].[BlogPost] add constraint [FK_BlogPost_BlogID_Blog_ID] foreign key ([BlogID]) references [b].[Blog] ([ID]);
alter table [bp].[BlogPost] add constraint [FK_BlogPost_AuthorID_Author_ID] foreign key ([AuthorID]) references [a].[Author] ([ID]);
alter table [bptj].[BlogPostTagJunction] add constraint [FK3] foreign key ([BlogPostID]) references [bp].[BlogPost] ([ID]);
alter table [bptj].[BlogPostTagJunction] add constraint [FK4] foreign key ([TagID]) references [t].[Tag] ([ID]);
create table [bpc].[BlogPostComment] (
[ID] int not null identity primary key,
[BlogPostID] int not null,
[AuthorEmailAddress] nvarchar(200) not null,
[Content] nvarchar(max) not null,
);
alter table [bpc].[BlogPostComment] add constraint [FK5] foreign key ([BlogPostID]) references [bp].[BlogPost] ([ID]);
create table [m].[Media] (
[ID] int not null identity primary key,
[TypeID] int ,
[Name] nvarchar(200) not null,
[MimeType] nvarchar(200) not null,
[Length] int ,
[BlogPostID] int null,
[BlogPostCommentID] int null,
);
create table [u].[User] (
[ID] int not null identity primary key,
[Login] nvarchar(200) not null,
[Password] varbinary(64) not null,
);
alter table [m].[Media] add constraint [FK10] foreign key ([BlogPostID]) references [bp].[BlogPost] ([ID]);
alter table [m].[Media] add constraint [FK11] foreign key ([BlogPostCommentID]) references [bpc].[BlogPostComment] ([ID]);
create unique nonclustered index [IX_Login] on [u].[User] ([ID], [Login] desc);
create table [f].[Forum] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
[ModeratorUserID] int not null,
);
alter table [f].[Forum] add constraint [FK_FOO] foreign key ([ModeratorUserID]) references [u].[User] ([ID]);
drop index [IX_Login] on [u].[User];
create unique nonclustered index [IX_Login] on [u].[User] ([ID], [Login] desc);
create table [baj].[BlogAuthorJunction] (
[BlogID] int not null,
[AuthorID] int not null,
);
alter table [baj].[BlogAuthorJunction] add constraint [FK12] foreign key ([BlogID]) references [b].[Blog] ([ID]);
alter table [baj].[BlogAuthorJunction] add constraint [FK13] foreign key ([AuthorID]) references [a].[Author] ([ID]);
alter table [f].[Forum] add [Slug] nvarchar(200) not null;
alter table [f].[Forum] drop column [Slug];
alter table [f].[Forum] add [Slug] nvarchar(200) not null;
alter table [f].[Forum] alter column [Slug] nvarchar(200) null;
alter table [f].[Forum] drop constraint [FK_FOO];
drop table [f].[Forum];
alter table [bptj].[BlogPostTagJunction] drop constraint [FK4];
drop table [t].[Tag];
", stringBuilder.ToString());
        }

        [Test()]
        public void GenerateScript3()
        {
            IAstNode astNode;
            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.BlogWithTemplates.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();
            } // using

            IDbPlatform platform = new SqlServer2000Platform();
            IMdlCompiler compiler = new MdlCompiler(new NullCodeGenerator(), new Environment());

            compiler.RemoveCompilerStage<DowngradeGenerationStage>();
            compiler.RemoveCompilerStage<UpgradeGenerationStage>();
            compiler.AddCompilerStageAfter<AstFlattenerCompilerStage>(new DbNamingCompilerStage(platform.NamingStrategy));
            compiler.Compile(astNode, MdlCompilationOptions.All);

            StringBuilder stringBuilder = new StringBuilder();


            IDbScriptGenerator scriptGenerator = platform.Dialect.CreateScriptGenerator(new StringWriter(stringBuilder));

            foreach (IVersionNode versionNode in Algorithms.Filter<IAstNode, IVersionNode>(astNode.ChildNodes))
                versionNode.Accept(scriptGenerator);

            Assert.AreEqual(@"create table [SchemaInfo] (
[Version] bigint not null,
);
create table [Author] (
[ID] int not null identity primary key,
[FirstName] nvarchar(200) not null,
[LastName] nvarchar(200) not null,
[EmailAddress] nvarchar(200) not null,
[Login] nvarchar(200) not null,
[Password] varbinary(64) null,
);
create table [Tag] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
);
create table [Blog] (
[ID] int not null identity primary key,
[Description] nvarchar(max) not null,
[Name] nvarchar(200) not null,
);
create table [BlogPost] (
[ID] int not null identity primary key,
[Title] nvarchar(200) not null,
[Slug] nvarchar(200) not null,
[AuthorID] int not null,
[BlogID] int not null,
);
create table [BlogPostTagJunction] (
[BlogPostID] int not null,
[TagID] int not null,
);
create unique nonclustered index [UQ_Version] on [SchemaInfo] ([Version]);
create unique nonclustered index [IX_EmailAddress] on [Author] ([EmailAddress]);
create unique nonclustered index [IX_Login] on [Author] ([Login]);
alter table [BlogPost] add constraint [FK2] foreign key ([AuthorID]) references [Author] ([ID]);
alter table [BlogPost] add constraint [FK_BlogPost_BlogID_Blog_ID] foreign key ([BlogID]) references [Blog] ([ID]);
alter table [BlogPostTagJunction] add constraint [FK3] foreign key ([BlogPostID]) references [BlogPost] ([ID]);
alter table [BlogPostTagJunction] add constraint [FK4] foreign key ([TagID]) references [Tag] ([ID]);
create table [BlogPostComment] (
[ID] int not null identity primary key,
[BlogPostID] int not null,
[AuthorEmailAddress] nvarchar(200) not null,
[Content] nvarchar(max) not null,
);
alter table [BlogPostComment] add constraint [FK5] foreign key ([BlogPostID]) references [BlogPost] ([ID]);
create table [Media] (
[ID] int not null identity primary key,
[TypeID] int ,
[MimeType] nvarchar(200) not null,
[Length] int ,
[BlogPostID] int null,
[BlogPostCommentID] int null,
[Name] nvarchar(200) not null,
);
create table [User] (
[ID] int not null identity primary key,
[Login] nvarchar(200) not null,
[Password] varbinary(64) not null,
);
alter table [Media] add constraint [FK10] foreign key ([BlogPostID]) references [BlogPost] ([ID]);
alter table [Media] add constraint [FK11] foreign key ([BlogPostCommentID]) references [BlogPostComment] ([ID]);
create unique nonclustered index [IX_Login] on [User] ([ID], [Login] desc);
create table [Forum] (
[ID] int not null identity primary key,
[Name] nvarchar(200) not null,
[ModeratorUserID] int not null,
);
alter table [Forum] add constraint [FK_FOO] foreign key ([ModeratorUserID]) references [User] ([ID]);
drop index [IX_Login] on [User];
create unique nonclustered index [IX_Login] on [User] ([ID], [Login] desc);
create table [BlogAuthorJunction] (
[BlogID] int not null,
[AuthorID] int not null,
);
alter table [BlogAuthorJunction] add constraint [FK12] foreign key ([BlogID]) references [Blog] ([ID]);
alter table [BlogAuthorJunction] add constraint [FK13] foreign key ([AuthorID]) references [Author] ([ID]);
alter table [Forum] add [Slug] nvarchar(200) not null;
alter table [Forum] drop column [Slug];
alter table [Forum] add [Slug] nvarchar(200) not null;
alter table [Forum] alter column [Slug] nvarchar(200) null;
alter table [Forum] drop constraint [FK_FOO];
drop table [Forum];
alter table [BlogPostTagJunction] drop constraint [FK4];
drop table [Tag];
", stringBuilder.ToString());
        }
    }
}
