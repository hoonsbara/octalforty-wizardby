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
using System.Text;

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Performs lexical analysis of MDL source files.
    /// </summary>
    public class MdlScanner : IMdlScanner
    {
        #region Private Constants
        private static readonly char[] SymbolCharacters = "._-".ToCharArray();
        #endregion

        #region Private Fields
        private readonly SourceReader sourceReader;
        private readonly IList<string> keywords = new List<string>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MdlScanner"/> class.
        /// </summary>
        /// <param name="sourceReader"></param>
        public MdlScanner(SourceReader sourceReader)
        {
            if(sourceReader == null) 
                throw new ArgumentNullException("sourceReader");

            this.sourceReader = sourceReader;
        }

        /// <summary>
        /// Registers a keyword <paramref name="keyword"/> within the scanner.
        /// </summary>
        /// <param name="keyword"></param>
        public void RegisterKeyword(string keyword)
        {
            if(keyword == null) 
                throw new ArgumentNullException("keyword");

            if(keyword == String.Empty)
                throw new ArgumentException("", "keyword");

            InternalRegisterKeyword(keyword);
        }

        /// <summary>
        /// Scans the input stream and returns scanned <see cref="Token"/> objects.
        /// </summary>
        /// <returns></returns>
        public TokenSequence Scan()
        {
            TokenSequence tokens = new TokenSequence();

            //
            // Scan indents in the beginning
            if(!sourceReader.Empty)
                ScanIndent(tokens);

            //
            // Main loop
            while(!sourceReader.Empty)
            {
                ScanInternal(tokens);
            } // while

            //
            // Terminate statements, if any
            if(tokens.Count > 0 && tokens.Last.Type != TokenType.EndStatement)
                tokens.Add(new Token(TokenType.EndStatement, null));

            //
            // Now postprocess what we have scanned so far and
            // remove extraneous Indents & stuff
            tokens = PostProcessTokens(new TokenSequence(tokens));

            return tokens;
        }

        private static TokenSequence PostProcessTokens(TokenSequence tokens)
        {
            return LayoutTokens(OptimizeTokens(tokens));
        }

        private static TokenSequence LayoutTokens(TokenSequence tokens)
        {
            TokenSequence laidOutTokens = new TokenSequence();

            Stack<int> indents = new Stack<int>();
            indents.Push(0);

            while(!tokens.Empty)
            {
                Token token = tokens.RemoveFirst();

                if(token.Type == TokenType.Indent)
                {
                    if(token.Lexeme.Length == indents.Peek())
                        continue;

                    if(token.Lexeme.Length > indents.Peek())
                    {
                        indents.Push(token.Lexeme.Length);
                        laidOutTokens.Add(new Token(TokenType.BeginBlock, token.Location));
                    } // if
                    else
                    {
                        while(indents.Peek() != 0 && indents.Peek() != token.Lexeme.Length)
                        {
                            indents.Pop();
                            laidOutTokens.Add(new Token(TokenType.EndBlock, token.Location));
                        } // while
                    } // else
                } // if
                else 
                    laidOutTokens.Add(token);
            } // while

            //
            // Close remaining blocks
            while(indents.Peek() != 0)
            {
                indents.Pop();
                laidOutTokens.Add(new Token(TokenType.EndBlock, null));
            } // while

            return laidOutTokens;
        }

        private static TokenSequence OptimizeTokens(TokenSequence tokens)
        {
            TokenSequence optimizedTokens = new TokenSequence();

            //
            // Removing sequences of Indent(EndStatement)+ tokens
            // and compacting EndStatement+ tokens to just one
            while(!tokens.Empty)
            {
                Token token = tokens.RemoveFirst();

                if(token.Type == TokenType.Indent && !tokens.Empty && tokens.First.Type == TokenType.EndStatement)
                    while(!tokens.Empty && tokens.First.Type == TokenType.EndStatement)
                        tokens.RemoveFirst();
                else if(token.Type == TokenType.EndStatement && !tokens.Empty && tokens.First.Type == TokenType.EndStatement)
                {
                    while(!tokens.Empty && tokens.First.Type == TokenType.EndStatement)
                        tokens.RemoveFirst();

                    optimizedTokens.Add(new Token(TokenType.EndStatement, null));
                } // else if
                else
                    optimizedTokens.Add(token);
            } // while

            return optimizedTokens;
        }

        private void ScanIndent(TokenSequence tokens)
        {
            int indent = 0;

            while(!sourceReader.Empty && (sourceReader.Next == ' ' || sourceReader.Next == '\t' ))
            {
                if(sourceReader.Next == ' ')
                    indent += 1;
                else if(sourceReader.Next == '\t')
                    indent += 4;

                sourceReader.ReadNext();
            } // while

            if(indent == 0)
                return;

            tokens.Add(new Token(TokenType.Indent, new StringBuilder().Insert(0, " ", indent).ToString(), 
                new Location(sourceReader.Line, sourceReader.Column + 1)));

            //
            // Return if we reached the end of the line - nothing to do here
            /*if(!sourceReader.Empty && sourceReader.Next == '\r')
                return;

            if(indents.Count == 0 || indents.Peek() < indent)
            {
                if(indents.Count == 0 || tokens.Count == 0)
                {
                    indents.Push(indent);
                    tokens.Add(new Token(TokenType.BeginBlock, null));
                } // if
                else
                {
                    if(tokens.Last.Type == TokenType.BeginBlock)
                    {
                        if(indents.Peek() != indent)
                        {
                            indents.Pop();
                            indents.Push(indent);
                        } // if
                    } // if
                    else
                    {
                        indents.Push(indent);
                        tokens.Add(new Token(TokenType.BeginBlock, null));
                    } // else
                } // else
            } // if
            else if(indents.Peek() > indent)
            {
                while(indents.Count != 0 && indents.Peek() != indent)
                {
                    indents.Pop();
                    tokens.Add(new Token(TokenType.EndBlock, null));
                } // while
            } // else if
             * */
        }

        private void ScanInternal(TokenSequence tokens)
        {
            char lookahead = sourceReader.Next;

            if(char.IsDigit(lookahead) || lookahead == '-')
                tokens.Add(ScanIntegerConstant());
            else if(lookahead == '"')
                tokens.Add(ScanStringConstant());
            else if(char.IsLetter(lookahead))
                tokens.Add(ScanSymbolOrKeyword());
            else if(lookahead == ':')
                tokens.Add(ScanColon());
            else if(lookahead == '=')
                tokens.Add(ScanPropertyAssignment());
            else if(lookahead == '[')
                tokens.Add(ScanLeftSquareBracket());
            else if(lookahead == ']')
                tokens.Add(ScanRightSquareBracket());
            else if(lookahead == '{')
                tokens.Add(ScanLeftBrace());
            else if(lookahead == '}')
                tokens.Add(ScanRightBrace());
            else if(lookahead == '(')
                tokens.Add(ScanLeftBracket());
            else if(lookahead == ')')
                tokens.Add(ScanRightBracket());
            else if(lookahead == ',')
                tokens.Add(ScanComma());
            else if(lookahead == '/')
                ScanComment();
            else if(lookahead == '\r')
            {
                tokens.Add(new Token(TokenType.EndStatement, new Location(sourceReader.Line, sourceReader.Column + 1)));

                sourceReader.ReadNext();
                sourceReader.ReadNext();

                ScanIndent(tokens);
            } // else if
            else if(!sourceReader.Empty)
                sourceReader.ReadNext();
        }

        private void ScanComment()
        {
            if(sourceReader.Next == '/')
                sourceReader.ReadNext();

            if(sourceReader.Next == '*')
                sourceReader.ReadNext();
                
            //
            // Scan comment. We're using previousChar to track stuff like */
            char? previousChar = null;
            while(!sourceReader.Empty)
            {
                //
                // Process nested comments
                if(sourceReader.Next == '*' && previousChar == '/')
                    ScanComment();
                else if(sourceReader.Next == '/' && previousChar == '*')
                {
                    //
                    // Consume the trailing /
                    sourceReader.ReadNext();
                    break;
                } // else if

                previousChar = sourceReader.ReadNext();
            } // while
        }

        private Token ScanLeftBracket()
        {
            return ScanBracket(TokenType.LeftBracket);
        }

        private Token ScanRightBracket()
        {
            return ScanBracket(TokenType.RightBracket);
        }

        private Token ScanComma()
        {
            return Scan(TokenType.Comma, SequenceScanner(1));
        }

        private Token ScanLeftBrace()
        {
            return ScanBracket(TokenType.LeftBrace);
        }

        private Token ScanRightBrace()
        {
            return ScanBracket(TokenType.RightBrace);
        }

        private Token ScanRightSquareBracket()
        {
            return ScanBracket(TokenType.RightSquareBracket);
        }

        private Token ScanLeftSquareBracket()
        {
            return ScanBracket(TokenType.LeftSquareBracket);
        }

        private Token ScanBracket(TokenType bracketType)
        {
            return Scan(bracketType, SequenceScanner(1));
        }

        private Token ScanPropertyAssignment()
        {
            return Scan(TokenType.PropertyAssignment, SequenceScanner(2));
        }

        private Token ScanColon()
        {
            return Scan(TokenType.Colon, SequenceScanner(1));
        }

        private Token ScanSymbolOrKeyword()
        {
            Token token = Scan(TokenType.Symbol, SymbolOrKeywordScanner());

            //
            // If the lexeme of the parsed symbol is in the registered keywords list, treat
            // is as keyword.
            return keywords.IndexOf(token.Lexeme) > -1 ? 
                new Token(TokenType.Keyword, token.Lexeme, token.Location) : 
                token;
        }

        private Token ScanStringConstant()
        {
            return Scan(TokenType.StringConstant, StringConstantScanner());
        }

        private Token ScanIntegerConstant()
        {
            return Scan(TokenType.IntegerConstant, IntegerConstantScanner());
        }

        private IEnumerable<char> SequenceScanner(int sequenceLength)
        {
            while(!sourceReader.Empty && sequenceLength-- != 0)
                yield return sourceReader.ReadNext();
        }

        private IEnumerable<char> SymbolOrKeywordScanner()
        {
            while(!sourceReader.Empty && (char.IsLetterOrDigit(sourceReader.Next) || IsOneOfSymbolCharacters(sourceReader.Next)))
                yield return sourceReader.ReadNext();
        }

        private static bool IsOneOfSymbolCharacters(char next)
        {
            return Array.IndexOf(SymbolCharacters, next) > -1;
        }

        private IEnumerable<char> StringConstantScanner()
        {
            //
            // Strip out first quote
            sourceReader.ReadNext();

            //
            // Scan string constant. We're using previousChar to track stuff like \"
            char? previousChar = null;
            while(!sourceReader.Empty && (sourceReader.Next != '"' || (sourceReader.Next == '"' && previousChar == '\\')))
                yield return (char)(previousChar = sourceReader.ReadNext());

            //
            // And finally strip the last quote
            sourceReader.ReadNext();
        }

        private IEnumerable<char> IntegerConstantScanner()
        {
            //
            // Watch out for negative integers
            if(sourceReader.Next == '-')
                yield return sourceReader.ReadNext();

            //
            // Scan integer constant
            while(!sourceReader.Empty && char.IsDigit(sourceReader.Next))
                yield return sourceReader.ReadNext();
        }

        private Token Scan(TokenType type, IEnumerable<char> lexeme)
        {
            Location location = new Location(sourceReader.Line, sourceReader.Column + 1);
            string lexemer = ScanLexeme(lexeme);

            return new Token(type, lexemer, location);
        }

        private static string ScanLexeme(IEnumerable<char> lexeme)
        {
            StringBuilder lexemeBuilder = new StringBuilder(20);
            foreach(char c in lexeme)
                lexemeBuilder.Append(c);

            return lexemeBuilder.ToString();
        }

        private void InternalRegisterKeyword(string keyword)
        {
            if(!keywords.Contains(keyword))
                keywords.Add(keyword);
        }
    }
}
