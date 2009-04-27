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

using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Util
{
    public static class Algorithms
    {
        public static IEnumerable<T> Reverse<T>(IEnumerable<T> source)
        {
            List<T> list = new List<T>(source);
            for(int i = list.Count - 1; i >= 0; --i)
                yield return list[i];
        }

        public static T[] ToArray<T>(IEnumerable<T> source)
        {
            return new List<T>(source).ToArray();
        }

        public static IEnumerable<U> Filter<T, U>(IEnumerable<T> source)
            where U : T
        {
            foreach(T t in source)
                if(t is U)
                    yield return (U)t;
        }

        public static T FindFirst<T>(IEnumerable<T> source, Predicate<T> condition)
        {
            foreach(T t in source)
                if(condition(t))
                    return t;

            return default(T);
        }

        public static string Join<T>(string separator, IEnumerable<T> source, Converter<T, string> converter)
        {
            StringBuilder joinBuilder = new StringBuilder();

            foreach(T value in source)
            {
                if(joinBuilder.Length > 0)
                    joinBuilder.Append(separator);

                joinBuilder.Append(converter(value));
            } // foreach

            return joinBuilder.ToString();
        }

        public static int LastIndexOf<T>(IList<T> list, Predicate<T> predicate)
        {
            for(int i = list.Count - 1; i >= 0 ; --i)
                if(predicate(list[i]))
                    return i;

            return -1;
        }

        public static int FirstIndexOf<T>(IList<T> list, Predicate<T> predicate)
        {
            for(int i = 0; i < list.Count; ++i)
                if(predicate(list[i]))
                    return i;

            return -1;
        }
    }
}
