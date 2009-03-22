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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    /// <summary>
    /// Represents an AST Node Property Collection.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    internal class AstNodePropertyCollection : IAstNodePropertyCollection
    {
        #region Private Fields
        private readonly IDictionary<string, IAstNodeProperty> properties = new Dictionary<string, IAstNodeProperty>();
        #endregion
                
        /// <summary>
        /// Initializes a new instance of the <see cref="AstNodePropertyCollection"/> class.
        /// </summary>
        public AstNodePropertyCollection()
        {
        }

        #region IAstNodePropertyCollection Members
        /// <summary>
        /// Gets an <see cref="IAstNodeProperty"/> object with a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IAstNodeProperty this[string name]
        {
            get { return properties[name]; }
        }

        /// <summary>
        /// Gets an value which contains the number of properties in the current collection.
        /// </summary>
        public int Count
        {
            get { return properties.Count; }
        }

        /// <summary>
        /// Adds a given <paramref name="property"/> to the current collection.
        /// </summary>
        /// <param name="property"></param>
        public void AddProperty(IAstNodeProperty property)
        {
            properties[property.Name] = property;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}"></see> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IAstNodeProperty> IEnumerable<IAstNodeProperty>.GetEnumerator()
        {
            foreach(IAstNodeProperty property in properties.Values)
                yield return property;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<IAstNodeProperty>)this).GetEnumerator();
        }

        /// <summary>
        /// Returns a value which indicates whether this collection contains property named <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsProperty(string name)
        {
            return properties.ContainsKey(name);
        }
        #endregion
    }
}
