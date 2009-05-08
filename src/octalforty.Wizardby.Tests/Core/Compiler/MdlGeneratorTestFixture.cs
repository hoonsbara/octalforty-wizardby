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

namespace octalforty.Wizardby.Tests.Core.Compiler
{
    [TestFixture()]
    public class MdlGeneratorTestFixture
    {
        [Test()]
        public void Generate()
        {
            IAstNode astNode;

            IMdlParser mdlParser;
            using(Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Oxite.mdl"))
            {
                mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();
            } // using
            
            MdlGenerator mdlGenerator = new MdlGenerator();

            StringBuilder mdlBuilder = new StringBuilder();
            mdlGenerator.Generate(astNode, new StringWriter(mdlBuilder));

            Assert.AreEqual(@"migration ""Oxite"" revision => 1:
    type-aliases:
        type-alias PK type => Guid, nullable => false
        type-alias LongName type => String, length => 256, nullable => false
        type-alias MediumName type => String, length => 128, nullable => false
        type-alias ShortName type => String, length => 100, nullable => false
    version 20090323103239:
        add table oxite_Language:
            add column LanguageID type => Guid, nullable => false, primary-key => true
            add column LanguageName type => AnsiString, length => 8, nullable => false
            add column LanguageDisplayName type => String, length => 50, nullable => false
    version 20090330170528:
        add table oxite_User:
            add column UserID type => PK, primary-key => true
            add column Username type => LongName, unique => true
            add column DisplayName type => LongName
            add column Email type => LongName
            add column HashedEmail type => ShortName
            add column Password type => MediumName
            add column PasswordSalt type => MediumName
            add column DefaultLanguageID references => oxite_Language
            add column Status type => Byte, nullable => false
        add table oxite_UserLanguage:
            add column UserID references => oxite_User
            add column LanguageID references => oxite_Language
            add index """" columns => [UserID, LanguageID], unique => true, clustered => true
            add index """" column => [UserID, asc]
    version 20090331135627:
        add table oxite_Role:
            add column RoleID type => PK, primary-key => true
            add column ParentRoleID references => oxite_Role, nullable => true
            add column RoleName type => LongName
        add table oxite_UserRoleRelationship:
            add column UserID references => oxite_User
            add column RoleID references => oxite_Role
    version 20090331140131:
        add table oxite_FileResource:
            add column FileResourceID type => PK, primary-key => true
            add column SiteID type => Guid, nullable => false
            add column FileResourceName type => LongName
            add column CreatorUserID references => oxite_User
            add column Data type => Binary
            add column ContentType type => AnsiString, length => 25, nullable => false
            add column Path type => String, length => 1000, nullable => false
            add column State type => Byte, nullable => false
            add column CreatedDate type => DateTime, nullable => false, default => ""getdate()""
            add column ModifiedDate type => DateTime, nullable => false:
                add constraint """" default => ""getdate()""
        add table oxite_UserFileResourceRelationship:
            add column UserID references => oxite_User
            add column FileResourceID references => oxite_FileResource:
                add index """" unique => true
            add index """" columns => [UserID, FileResourceID], unique => true, clustered => true
", mdlBuilder.ToString());

            mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StringReader(mdlBuilder.ToString())));
            IAstNode reparsedNode = mdlParser.Parse();
        }
    }
}
