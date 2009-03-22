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
namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// Provides a base implementation of <see cref="ISchemaElementDefinition"/>.
    /// </summary>
    public abstract class SchemaElementDefinitionBase : ISchemaElementDefinition
    {
        #region Private Fields
        private string name;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaElementDefinitionBase"/> class.
        /// </summary>
        protected SchemaElementDefinitionBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaElementDefinitionBase"/> class.
        /// </summary>
        /// <param name="name"></param>
        protected SchemaElementDefinitionBase(string name)
        {
            this.name = name;
        }

        #region ISchemaElementDefinition Members
        /// <summary>
        /// Gets or sets a string which contains the name of the current schema element.
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

    }
}
