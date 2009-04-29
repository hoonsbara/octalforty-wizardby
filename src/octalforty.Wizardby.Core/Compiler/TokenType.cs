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
namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Defines the type of a <see cref="Token"/>.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Integer constant.
        /// </summary>
        IntegerConstant,

        /// <summary>
        /// AsString constant.
        /// </summary>
        StringConstant,

        /// <summary>
        /// Symbol.
        /// </summary>
        Symbol,

        /// <summary>
        /// Colon symbol "<c>:</c>".
        /// </summary>
        Colon,

        /// <summary>
        /// Property Assignment "<c>=></c>".
        /// </summary>
        PropertyAssignment,

        /// <summary>
        /// Left Square Bracket "<c>[</c>".
        /// </summary>
        LeftSquareBracket,

        /// <summary>
        /// Right Square Bracket "<c>]</c>".
        /// </summary>
        RightSquareBracket,

        /// <summary>
        /// Left Brace "<c>{</c>".
        /// </summary>
        LeftBrace,

        /// <summary>
        /// Right Brace "<c>}</c>".
        /// </summary>
        RightBrace,

        /// <summary>
        /// Comma "<c>,</c>".
        /// </summary>
        Comma,

        /// <summary>
        /// Left Bracket "<c>(</c>".
        /// </summary>
        LeftBracket,

        /// <summary>
        /// Right Bracket "<c>)</c>".
        /// </summary>
        RightBracket,

        /// <summary>
        /// Keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// Beginning of the Block.
        /// </summary>
        BeginBlock,

        /// <summary>
        /// End of the Block.
        /// </summary>
        EndBlock,

        /// <summary>
        /// End of the Statement.
        /// </summary>
        EndStatement,

        /// <summary>
        /// Indent.
        /// </summary>
        Indent
    }
}
