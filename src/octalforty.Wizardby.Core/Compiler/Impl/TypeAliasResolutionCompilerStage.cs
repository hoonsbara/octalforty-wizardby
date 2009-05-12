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
    public class TypeAliasResolutionCompilerStage : MdlCompilerStageBase
    {
        #region Private Constants
        private readonly string[] MergeablePropertyNames = new string[]
            { MdlSyntax.Type, MdlSyntax.Length, MdlSyntax.Nullable, MdlSyntax.Scale, MdlSyntax.Precision, MdlSyntax.Default };
        #endregion

        #region Private Fields
        private readonly IDictionary<string, ITypeAliasNode> typeAliases = new Dictionary<string, ITypeAliasNode>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAliasResolutionCompilerStage"/> class.
        /// </summary>
        public TypeAliasResolutionCompilerStage()
        {
        }

        #region MdlCompilerStageBase Members
        /// <summary>
        /// Visits the given <paramref name="typeAliasNode"/>.
        /// </summary>
        /// <param name="typeAliasNode"></param>
        public override void Visit(ITypeAliasNode typeAliasNode)
        {
            //
            // Collect all Type Aliases
            typeAliases[typeAliasNode.Name] = typeAliasNode;
        }

        /// <summary>
        /// Visits the given <paramref name="defaultPrimaryKeyNode"/>.
        /// </summary>
        /// <param name="defaultPrimaryKeyNode"></param>
        public override void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode)
        {
            ResolveTypeAlias(defaultPrimaryKeyNode);
        }

        /// <summary>
        /// Visits the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        public override void Visit(IAddColumnNode addColumnNode)
        {
            ResolveTypeAlias(addColumnNode);
        }
        #endregion

        private void ResolveTypeAlias(IAstNode astNode)
        {
            //
            // No type specified or type name is not an alias -- nothing to do here. 
            if(astNode.Properties[MdlSyntax.Type] == null ||
               !typeAliases.ContainsKey(AstNodePropertyUtil.AsString(astNode.Properties[MdlSyntax.Type].Value)))
                return;

            ITypeAliasNode typeAlias = GetTypeAlias(AstNodePropertyUtil.AsString(astNode.Properties[MdlSyntax.Type].Value));
                
            //
            // Merge type specification
            MergeProperties(astNode, typeAlias);
        }

        private void MergeProperties(IAstNode targetAstNode, IAstNode sourceAstNode)
        {
            foreach(IAstNodeProperty property in MergeableProperties(sourceAstNode.Properties))
                targetAstNode.Properties.AddProperty(property);
        }

        private ITypeAliasNode GetTypeAlias(string name)
        {
            ITypeAliasNode mergedTypeAlias = new TypeAliasNode(null, name);

            IList<ITypeAliasNode> typeAliasesChain = GetTypeAliasesChain(name);

            foreach(ITypeAliasNode typeAlias in typeAliasesChain)
                MergeProperties(mergedTypeAlias, typeAlias);

            //
            // And we need to preserve type specification
            mergedTypeAlias.Properties.AddProperty(typeAliasesChain[0].Properties[MdlSyntax.Type]);
            return mergedTypeAlias;
        }

        private IEnumerable<ITypeAliasNode> GetTypeAliasesRecursive(string name)
        {
            if(!typeAliases.ContainsKey(name))
                yield break;

            ITypeAliasNode typeAlias = typeAliases[name];
            yield return typeAlias;

            if(typeAlias.Properties[MdlSyntax.Type] != null)
                foreach(ITypeAliasNode parentTypeAlias in GetTypeAliasesRecursive(AstNodePropertyUtil.AsString(typeAlias.Properties[MdlSyntax.Type].Value)))
                    yield return parentTypeAlias;
        }

        private IList<ITypeAliasNode> GetTypeAliasesChain(string name)
        {
            List<ITypeAliasNode> typeAliasesChain = new List<ITypeAliasNode>(GetTypeAliasesRecursive(name));
            typeAliasesChain.Reverse();
            
            return typeAliasesChain;
        }

        private IEnumerable<IAstNodeProperty> MergeableProperties(IEnumerable<IAstNodeProperty> properties)
        {
            foreach(IAstNodeProperty property in properties)
                if(Array.IndexOf(MergeablePropertyNames, property.Name) > -1)
                    yield return property;
        }
    }
}
