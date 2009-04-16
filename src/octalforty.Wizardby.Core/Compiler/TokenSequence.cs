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

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Represents a sequence of tokens.
    /// </summary>
    public class TokenSequence
    {
        #region Private Fields
        public readonly LinkedList<Token> tokens = new LinkedList<Token>();
        #endregion
        
        #region Public Properties
        /// <summary>
        /// Gets a value which indicates whether current token sequence is empty.
        /// </summary>
        public bool Empty
        {
            get { return tokens.Count == 0; }
        }

        /// <summary>
        /// Gets a <see cref="Token"/> object which represents the first token in the
        /// current token sequence.
        /// </summary>
        public Token First
        {
            get
            {
                if(Empty)
                    throw new InvalidOperationException();

                return InternalFirst();
            }
        }

        /// <summary>
        /// Gets a <see cref="Token"/> object which represents the last token in the
        /// current token sequence.
        /// </summary>
        public Token Last
        {
            get
            {
                if(Empty)
                    throw new InvalidOperationException();

                return InternalLast();
            }
        }

        /// <summary>
        /// Gets a value which contains the number of tokens in the current token sequence.
        /// </summary>
        public int Count
        {
            get { return tokens.Count; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSequence"/> class.
        /// </summary>
        public TokenSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSequence"/> class.
        /// </summary>
        /// <param name="tokens"></param>
        public TokenSequence(TokenSequence tokens)
        {
            foreach(Token token in tokens.tokens)
                Add(token);
        }

        /// <summary>
        /// Adds a token <paramref name="token"/> to the current token sequence.
        /// </summary>
        /// <param name="token"></param>
        public void Add(Token token)
        {
            tokens.AddLast(token);
        }

        /// <summary>
        /// Removes and returns the first token of the current token sequence.
        /// </summary>
        /// <returns></returns>
        public Token RemoveFirst()
        {
            Token first = First;
            tokens.RemoveFirst();

            return first;
        }

        /// <summary>
        /// Inserts <paramref name="token"/> to the very beginning of the sequence.
        /// </summary>
        /// <param name="token"></param>
        public void InsertFirst(Token token)
        {
            tokens.AddFirst(token);
        }

        private Token InternalLast()
        {
            return tokens.Last.Value;
        }

        private Token InternalFirst()
        {
            return tokens.First.Value;
        }
    }
}
