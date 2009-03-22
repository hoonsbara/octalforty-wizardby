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

namespace octalforty.Wizardby.Core.Refactoring
{
    /// <summary>
    /// Defines Refactoring metadata.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RefactoringAttribute : Attribute
    {
        #region Private Fields
        private readonly string name;
        private readonly string alias;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a <see cref="string"/> which contains a human-readable name of the Refactoring.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets a <see cref="string"/> which contains an alias for the current Refactoring.
        /// </summary>
        public string Alias
        {
            get { return alias; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RefactoringAttribute"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        public RefactoringAttribute(string name, string alias)
        {
            this.name = name;
            this.alias = alias;
        }
    }
}
