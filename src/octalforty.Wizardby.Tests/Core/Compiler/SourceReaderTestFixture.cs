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
using System.IO;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;

namespace octalforty.Wizardby.Tests.Core.Compiler
{
    [TestFixture()]
    public class SourceReaderTestFixture
    {
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextThrowsInvalidOperationExceptionOnEmptyReader()
        {
            SourceReader sourceReader = new SourceReader(new StringReader(string.Empty));
            char next = sourceReader.Next;
        }

        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadNextThrowsInvalidOperationExceptionOnEmptyReader()
        {
            SourceReader sourceReader = new SourceReader(new StringReader(string.Empty));
            sourceReader.ReadNext();
        }

        [Test()]
        public void ReadNextReadsAllSource()
        {
            string source = "1 2" + System.Environment.NewLine + "3456" + System.Environment.NewLine + "789";
            SourceReader sourceReader = new SourceReader(new StringReader(source));

            StringBuilder sourceBuilder = new StringBuilder();
            while(!sourceReader.Empty)
                sourceBuilder.Append(sourceReader.ReadNext());

            Assert.AreEqual(source, sourceBuilder.ToString());
        }

        [Test()]
        public void ReadNextUpdatesPosition()
        {
            string source = "1 2" + System.Environment.NewLine + "3456" + System.Environment.NewLine + "789";
            SourceReader sourceReader = new SourceReader(new StringReader(source));

            Assert.IsFalse(sourceReader.Empty);
            Assert.AreEqual(0, sourceReader.Line);
            Assert.AreEqual(-1, sourceReader.Column);

            Assert.AreEqual('1', sourceReader.ReadNext());
            Assert.AreEqual(0, sourceReader.Line);
            Assert.AreEqual(0, sourceReader.Column);

            Assert.AreEqual(' ', sourceReader.ReadNext());
            Assert.AreEqual('2', sourceReader.ReadNext());
            Assert.AreEqual('\r', sourceReader.ReadNext());
            Assert.AreEqual('\n', sourceReader.ReadNext());

            Assert.AreEqual('3', sourceReader.ReadNext());
            Assert.AreEqual(1, sourceReader.Line);
            Assert.AreEqual(0, sourceReader.Column);

            Assert.AreEqual('4', sourceReader.ReadNext());
            Assert.AreEqual(1, sourceReader.Line);
            Assert.AreEqual(1, sourceReader.Column);

            Assert.AreEqual('5', sourceReader.ReadNext());
            Assert.AreEqual('6', sourceReader.ReadNext());
            Assert.AreEqual('\r', sourceReader.ReadNext());
            Assert.AreEqual('\n', sourceReader.ReadNext());

            Assert.AreEqual('7', sourceReader.ReadNext());
            Assert.AreEqual(2   , sourceReader.Line);
            Assert.AreEqual(0, sourceReader.Column);
        }
    }
}
