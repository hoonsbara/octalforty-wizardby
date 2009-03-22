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

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Provides character-based access to a given <see cref="TextReader"/>.
    /// </summary>
    public class SourceReader
    {
        #region Private Fields
        private readonly TextReader textReader;
        private int line;
        private int column = -1;
        private char? previousChar; // For tracking newlines
        #endregion
        
        #region Public Properties
        /// <summary>
        /// Gets a value which contains a zero-based number of the current line.
        /// </summary>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets a value which contains a zero-based number of the current column.
        /// </summary>
        public int Column
        {
            get { return column; }
        }

        /// <summary>
        /// Gets a value which indicates whether this <see cref="SourceReader"/> is empty.
        /// </summary>
        public bool Empty
        {
            get { return textReader.Peek() == -1; }
        }

        /// <summary>
        /// Gets a <see cref="char"/> with the next value.
        /// </summary>
        public char Next
        {
            get
            {
                if(Empty)
                    throw new InvalidOperationException();

                return InternalNext();
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceReader"/> class.
        /// </summary>
        /// <param name="textReader"></param>
        public SourceReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        /// <summary>
        /// Reads and returns the next character from the input stream.
        /// </summary>
        /// <returns></returns>
        public char ReadNext()
        {
            if(Empty)
                throw new InvalidOperationException();

            char readNext = InternalReadNext();

            return readNext;
        }

        private char InternalReadNext()
        {
            if(previousChar == '\r' && Next == '\n')
            {
                line += 1;
                column = -1;
            } // if
            else
                column += 1;

            return (previousChar = (char?)textReader.Read()).Value;
        }

        private char InternalNext()
        {
            return (char)textReader.Peek();
        }
    }
}
