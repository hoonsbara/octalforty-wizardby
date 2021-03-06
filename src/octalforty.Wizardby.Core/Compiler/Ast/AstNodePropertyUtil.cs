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

using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    public static class AstNodePropertyUtil
    {
        public static string AsString(IAstNodePropertyCollection properties, string name)
        {
            // TODO: Changed exception type
            if (!(properties[name].Value is IStringAstNodePropertyValue))
                throw new InvalidOperationException();

            return AsString(properties[name].Value);
        }

        public static int AsInteger(IAstNodePropertyCollection properties, string name)
        {
            // TODO: Changed exception type
            if (!(properties[name].Value is IIntegerAstNodePropertyValue))
                throw new InvalidOperationException();

            return AsInteger(properties[name].Value);
        }

        public static string AsString(IAstNodePropertyValue propertyValue)
        {
            return ((IStringAstNodePropertyValue)propertyValue).Value;
        }

        public static int AsInteger(IAstNodePropertyValue propertyValue)
        {
            return ((IIntegerAstNodePropertyValue)propertyValue).Value;
        }

        public static IAstNodeProperty AsString(string name, string value)
        {
            return new AstNodeProperty(name, new StringAstNodePropertyValue(value));
        }
    }
}
