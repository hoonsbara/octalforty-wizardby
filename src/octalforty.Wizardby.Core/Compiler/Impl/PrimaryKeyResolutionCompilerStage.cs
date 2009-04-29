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

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    /// <summary>
    /// A MDL Compiler Stage which inserts primary key definitions to <see cref="IAddTableNode"/> nodes
    /// </summary>
    public class PrimaryKeyResolutionCompilerStage : MdlCompilerStageBase
    {
        #region Private Constants
        private readonly string[] MergeablePropertyNames = new string[] 
            { 
                MdlSyntax.Type, MdlSyntax.Length, MdlSyntax.Nullable, MdlSyntax.Scale, MdlSyntax.Precision, 
                MdlSyntax.PrimaryKey, MdlSyntax.Identity, MdlSyntax.Default 
            };
        #endregion

        #region Private Fields
        private IDefaultPrimaryKeyNode defaultPrimaryKeyNode;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyResolutionCompilerStage"/> class.
        /// </summary>
        public PrimaryKeyResolutionCompilerStage()
        {
        }

        #region MdlCompilerStageBase Members
        /// <summary>
        /// Visits the given <paramref name="defaultPrimaryKeyNode"/>.
        /// </summary>
        /// <param name="defaultPrimaryKeyNode"></param>
        public override void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode)
        {
            //
            // Add "primary-key" property
            defaultPrimaryKeyNode.Properties.AddProperty(AstNodeProperty.Symbol(MdlSyntax.PrimaryKey, "true"));
            this.defaultPrimaryKeyNode = defaultPrimaryKeyNode;
        }

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            //
            // If table has "primary-key" property set to "false", no not
            // resolve PK for it
            if(addTableNode.Properties.ContainsProperty(MdlSyntax.PrimaryKey) &&
                addTableNode.Properties[MdlSyntax.PrimaryKey].Value.ToString() == "false")
                return;

            //
            // Iterate through IAddColumnNode children to see if there's a PK specified. If there is one
            // no default PK should be added.
            foreach(IAddColumnNode addColumnNode in Filter<IAddColumnNode>(addTableNode.ChildNodes))
            {
                //
                // We can't use Environment.Resolve here since no bindings were 
                // built yet. Rely on raw properties instead
                if(addColumnNode.Properties.ContainsProperty(MdlSyntax.PrimaryKey) &&
                    addColumnNode.Properties[MdlSyntax.PrimaryKey].Value.ToString() == "true")
                    return;
            } // foreach

            if(defaultPrimaryKeyNode == null)
                return;

            //
            // Create an IAddColumnNode and copy all properties from the default PK...
            IAddColumnNode addPrimaryKeyColumnNode = new AddColumnNode(addTableNode, defaultPrimaryKeyNode.Name);
            foreach(IAstNodeProperty astNodeProperty in MergeableProperties(defaultPrimaryKeyNode.Properties))
                addPrimaryKeyColumnNode.Properties.AddProperty(astNodeProperty);

            //
            // Add it to the front...
            addTableNode.ChildNodes.Insert(0, addPrimaryKeyColumnNode);
        }
        #endregion

        private IEnumerable<IAstNodeProperty> MergeableProperties(IEnumerable<IAstNodeProperty> properties)
        {
            foreach (IAstNodeProperty property in properties)
                if (Array.IndexOf(MergeablePropertyNames, property.Name) > -1)
                    yield return property;
        }
    }
}
