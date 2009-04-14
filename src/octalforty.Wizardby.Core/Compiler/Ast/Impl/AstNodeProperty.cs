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
using System.Diagnostics;

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    /// <summary>
    /// Represents an AST Node Property.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    public class AstNodeProperty : IAstNodeProperty
    {
        #region Private Fields
        private readonly string name;
        private readonly object value;
        private Location location;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AstNodeProperty"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public AstNodeProperty(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AstNodeProperty"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="location"></param>
        public AstNodeProperty(string name, object value, Location location)
        {
            this.name = name;
            this.value = value;
            this.location = location;
        }

        #region IAstNodeProperty Members
        /// <summary>
        /// Gets a <see cref="string"/> which contains the name of the current property.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets an <see cref="object"/> which contains the value of the current property.
        /// </summary>
        public object Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="Compiler.Location"/> of this property
        /// in a source file.
        /// </summary>
        public Location Location
        {
            get { return location; }
            set { location = value; }
        }
        #endregion
    }
}
