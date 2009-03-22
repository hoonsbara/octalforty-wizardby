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
using System.Diagnostics;

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Represents a single token.
    /// </summary>
    [DebuggerDisplay("{Lexeme} ({Type})")]
    public class Token : IEquatable<Token>
    {
        #region Private Fields
        private readonly TokenType type;
        private readonly string lexeme;
        private readonly Location location;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a member of <see cref="TokenType"/> enumeration, which represents the type
        /// of the current token.
        /// </summary>
        public TokenType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets a string which contains the lexeme of the current token.
        /// </summary>
        public string Lexeme
        {
            get { return lexeme; }
        }

        /// <summary>
        /// Gets a <see cref="Compiler.Location"/> object which contains the location of the current token.
        /// </summary>
        public Location Location
        {
            get { return location; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lexeme"></param>
        /// <param name="location"></param>
        public Token(TokenType type, string lexeme, Location location)
        {
            this.type = type;
            this.location = location;
            this.lexeme = lexeme;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="location"></param>
        public Token(TokenType type, Location location) :
            this(type, string.Empty, location)
        {
        }

        #region IEquatable<Token> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        /// <param name="token">An object to compare with this object.</param>
        public bool Equals(Token token)
        {
            if (token == null)
                return false;

            return Equals(type, token.type) && Equals(lexeme, token.lexeme);
        }
        #endregion


        #region Object Members
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(this, obj)) 
                return true;
            
            return Equals(obj as Token);
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() + 29 * lexeme.GetHashCode();
        }
        
        public override string ToString()
        {
            return string.Format("{0} ({1})", Lexeme, Type);
        }
        #endregion
    }
}
