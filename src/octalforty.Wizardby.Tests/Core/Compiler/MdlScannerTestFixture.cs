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
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;

namespace octalforty.Wizardby.Tests.Core.Compiler
{
    [TestFixture()]
    public class MdlScannerTestFixture
    {
        [Test()]
        public void ScanComplexFile()
        {
            using(Stream resourceStream = 
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Waffle.mdl"))
            {
                IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StreamReader(resourceStream, Encoding.UTF8)));
                mdlScanner.RegisterKeyword("migration");
                mdlScanner.RegisterKeyword("default");
                mdlScanner.RegisterKeyword("create");
                mdlScanner.RegisterKeyword("table");

                TokenSequence tokens = mdlScanner.Scan();

                Assert.Greater(tokens.Count, 0);
            } // using
        }

        [Test()]
        public void ScanKeywords()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("migration \"Waffle\" revision => 1 default create table")));
            mdlScanner.RegisterKeyword("migration");
            mdlScanner.RegisterKeyword("default");
            mdlScanner.RegisterKeyword("create");
            mdlScanner.RegisterKeyword("table");

            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(9, tokens.Count);
            Assert.AreEqual(new Token(TokenType.Keyword, "migration", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.StringConstant, "Waffle", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "revision", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.PropertyAssignment, "=>", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "1", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Keyword, "default", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Keyword, "create", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Keyword, "table", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, null), tokens.RemoveFirst());

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterKeywordThrowsArgumentNullExceptionOnNullKeyword()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("migration \"Waffle\" revision => 1 default create table")));
            mdlScanner.RegisterKeyword(null);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterKeywordThrowsArgumentExceptionOnEmptyKeyword()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("migration \"Waffle\" revision => 1 default create table")));
            mdlScanner.RegisterKeyword("");
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionOnNullSourceReader()
        {
            new MdlScanner(null);
        }

        [Test()]
        public void ScanPunctuation()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader(": : => [[, ] ] {,} { } (( )")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(17, tokens.Count);
            Assert.AreEqual(new Token(TokenType.Colon, ":", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Colon, ":", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.PropertyAssignment, "=>", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.LeftSquareBracket, "[", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.LeftSquareBracket, "[", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Comma, ",", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.RightSquareBracket, "]", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.RightSquareBracket, "]", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.LeftBrace, "{", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Comma, ",", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.RightBrace, "}", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.LeftBrace, "{", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.RightBrace, "}", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.LeftBracket, "(", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.LeftBracket, "(", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.RightBracket, ")", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, null), tokens.RemoveFirst());

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        public void ScanIntegerConstants()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("123 -123 0 1")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "123", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "-123", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "0", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "1", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, null), tokens.RemoveFirst());

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        public void ScanStringConstants()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("\"abc\" \"cde\\\"fg\\\"h\" \"\"")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual(new Token(TokenType.StringConstant, "abc", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.StringConstant, "cde\\\"fg\\\"h", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.StringConstant, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, null), tokens.RemoveFirst());

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        public void ScanSymbols()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("high-velocity bullet_at close.range candamage")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual(new Token(TokenType.Symbol, "high-velocity", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "bullet_at", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "close.range", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "candamage", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, null), tokens.RemoveFirst());

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        public void ScanSymbolsWithMultilineComments()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("high-velocity /*bullet_at" + 
                System.Environment.NewLine + "close.*/range candamage")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual(new Token(TokenType.Symbol, "high-velocity", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "range", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "candamage", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, null), tokens.RemoveFirst());

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        public void ScanUpdatesLocations()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("\"abc\" \"cde\\\"fg\\\"h\" \"\"" +
                System.Environment.NewLine + "hit -123 0 1   ")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(9, tokens.Count);
            Assert.AreEqual(new Location(0, 0), tokens.RemoveFirst().Location);
            Assert.AreEqual(new Location(0, 6), tokens.RemoveFirst().Location);
            
            tokens.RemoveFirst();
            tokens.RemoveFirst();
            
            Assert.AreEqual(new Location(1, 0), tokens.RemoveFirst().Location);
            Assert.AreEqual(new Location(1, 4), tokens.RemoveFirst().Location);
        }

        [Test()]
        public void ScanNestedComments()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader("/* high-velocity /*bullet_/*a/*t*/*/ close.range*/ candamage*/")));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(0, tokens.Count);
        }

        [Test()]
        public void ScanBlocks()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader(
@"migration revision => 1:
    baseline:
        create table Hi
        create table There")));
            TokenSequence tokens = mdlScanner.Scan();
            
            Assert.AreEqual(21, tokens.Count);

            Assert.AreEqual(new Token(TokenType.Symbol, "migration", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "revision", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.PropertyAssignment, "=>", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "1", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Colon, ":", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.BeginBlock, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "baseline", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Colon, ":", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.BeginBlock, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "create", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "table", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "Hi", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "create", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "table", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "There", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndBlock, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndBlock, "", null), tokens.RemoveFirst());
        }

        [Test()]
        public void ScanBlocksWithWeirdLayout()
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(new StringReader(
@"migration revision => 1:" + System.Environment.NewLine +
"       " + System.Environment.NewLine +
@"    baseline:" + System.Environment.NewLine +
"           " + System.Environment.NewLine +
"       /*        */" + System.Environment.NewLine +
"   " + System.Environment.NewLine +
"        create table Hi" + System.Environment.NewLine +
"   " + System.Environment.NewLine +
"           " + System.Environment.NewLine +
"        create table There" + System.Environment.NewLine +
"" + System.Environment.NewLine +
"   " + System.Environment.NewLine +
"" + System.Environment.NewLine +
"" + System.Environment.NewLine)));
            TokenSequence tokens = mdlScanner.Scan();

            Assert.AreEqual(21, tokens.Count);

            Assert.AreEqual(new Token(TokenType.Symbol, "migration", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "revision", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.PropertyAssignment, "=>", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.IntegerConstant, "1", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Colon, ":", null), tokens.RemoveFirst());
            
            Token endStatementToken = tokens.RemoveFirst();
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), endStatementToken);
            Assert.Greater(endStatementToken.Location.Column, 0);

            Token beginBlockToken = tokens.RemoveFirst();
            Assert.AreEqual(new Token(TokenType.BeginBlock, "", null), beginBlockToken);
            Assert.Greater(beginBlockToken.Location.Column, 0);

            Assert.AreEqual(new Token(TokenType.Symbol, "baseline", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Colon, ":", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.BeginBlock, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "create", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "table", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "Hi", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "create", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "table", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.Symbol, "There", null), tokens.RemoveFirst());
            Assert.AreEqual(new Token(TokenType.EndStatement, "", null), tokens.RemoveFirst());
            
            Token endBlockToken = tokens.RemoveFirst();
            Assert.AreEqual(new Token(TokenType.EndBlock, "", null), endBlockToken);
            Assert.AreEqual(new Token(TokenType.EndBlock, "", null), tokens.RemoveFirst());
        }
    }
}
