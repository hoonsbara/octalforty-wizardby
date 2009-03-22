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
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    public static class AstUtil
    {
        public static IAddColumnNode Clone(IAddColumnNode addColumnNode)
        {
            IAddColumnNode node = new AddColumnNode(addColumnNode.Parent, addColumnNode.Name);
            node.Default = addColumnNode.Default;
            node.Identity = addColumnNode.Identity;
            node.Length = addColumnNode.Length;
            node.Location = addColumnNode.Location;
            node.Nullable = addColumnNode.Nullable;
            node.Precision = addColumnNode.Precision;
            node.PrimaryKey = addColumnNode.PrimaryKey;
            node.Scale = addColumnNode.Scale;
            node.Table = addColumnNode.Table;
            node.Type = addColumnNode.Type;

            foreach(IAstNodeProperty property in addColumnNode.Properties)
                node.Properties.AddProperty(new AstNodeProperty(property.Name, property.Value));

            return node;
        }

        public static IAstNode Clone(IAstNode astNode)
        {
            if(astNode is IAddColumnNode)
                return Clone((IAddColumnNode)astNode);

            return null;
        } 
    }
}
