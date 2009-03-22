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

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;

namespace octalforty.Wizardby.Tests.Core.Compiler
{
    [TestFixture()]
    public class TokenSequenceTestFixture
    {
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FirstThrowsInvalidOperationExceptionOnEmptySequence()
        {
            TokenSequence sequence = new TokenSequence();
            Token first = sequence.First;
        }

        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LastThrowsInvalidOperationExceptionOnEmptySequence()
        {
            TokenSequence sequence = new TokenSequence();
            Token last = sequence.Last;
        }

        [Test()]
        public void AddAndCount()
        {
            TokenSequence sequence = new TokenSequence();
            sequence.Add(new Token(TokenType.BeginBlock, null));
            sequence.Add(new Token(TokenType.Colon, null));
            sequence.Add(new Token(TokenType.Comma, null));

            Assert.AreEqual(3, sequence.Count);
        }

        [Test()]
        public void First()
        {
            TokenSequence sequence = new TokenSequence();
            sequence.Add(new Token(TokenType.BeginBlock, null));
            sequence.Add(new Token(TokenType.Colon, null));
            sequence.Add(new Token(TokenType.Comma, null));

            Assert.AreEqual(new Token(TokenType.BeginBlock, null), sequence.First);
        }

        [Test()]
        public void Last()
        {
            TokenSequence sequence = new TokenSequence();
            sequence.Add(new Token(TokenType.BeginBlock, null));
            sequence.Add(new Token(TokenType.Colon, null));
            sequence.Add(new Token(TokenType.Comma, null));

            Assert.AreEqual(new Token(TokenType.Comma, null), sequence.Last);
        }

        [Test()]
        public void RemoveFirst()
        {
            TokenSequence sequence = new TokenSequence();
            sequence.Add(new Token(TokenType.BeginBlock, null));
            sequence.Add(new Token(TokenType.Colon, null));
            sequence.Add(new Token(TokenType.Comma, null));

            Assert.AreEqual(new Token(TokenType.BeginBlock, null), sequence.RemoveFirst());
            Assert.AreEqual(2, sequence.Count);
        }
    }
}
