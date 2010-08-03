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

using System.Collections.Generic;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    /// <summary>
    /// Represents an indexable collection of <see cref="IAstNodeProperty"/> objects.
    /// </summary>
    public interface IAstNodePropertyCollection : IEnumerable<IAstNodeProperty>
    {
        /// <summary>
        /// Gets an <see cref="IAstNodeProperty"/> object with a given name or <c>null</c> if
        /// no property with the given name is defined.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAstNodeProperty this[string name]
        { get; }

        /// <summary>
        /// Gets an value which contains the number of properties in the current collection.
        /// </summary>
        int Count
        { get; }

        /// <summary>
        /// Adds a given <paramref name="property"/> to the current collection.
        /// </summary>
        /// <param name="property"></param>
        void AddProperty(IAstNodeProperty property);
    }
}
