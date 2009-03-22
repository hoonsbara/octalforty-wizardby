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
    /// Represents a location within a source file.
    /// </summary>
    [DebuggerDisplay("{Line}:{Column}")]
    public sealed class Location : IEquatable<Location>
    {
        #region Private Fields
        private readonly int line;
        private readonly int column;
        #endregion
        
        #region Public Properties
        /// <summary>
        /// Gets a value which contains a zero-based line number.
        /// </summary>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets a value which contains a zero-based column number.
        /// </summary>
        public int Column
        {
            get { return column; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        public Location(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        #region IEquatable<Location> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        /// <param name="location">An object to compare with this object.</param>
        public bool Equals(Location location)
        {
            if(location == null) 
                return false;
            
            return line == location.line && column == location.column;
        }
        #endregion

        #region Object Members
        public override bool Equals(object obj)
        {
            if(ReferenceEquals(this, obj)) 
                return true;
            
            return Equals(obj as Location);
        }

        public override int GetHashCode()
        {
            return line + 29 * column;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Line, Column);
        }
        #endregion
    }
}
