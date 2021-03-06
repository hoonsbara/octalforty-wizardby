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
using System.IO;
using System.Text;
using System.Linq;

namespace octalforty.Wizardby.Core.Util
{
    public static class StringUtil
    {
        public static string Join(string separator, IEnumerable<string> strings)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(string s in strings)
            {
                if(stringBuilder.Length > 0)
                    stringBuilder.Append(separator);

                stringBuilder.Append(s);
            } // foreach

            return stringBuilder.ToString();
        }
    }

    public static class StreamUtil
    {
        public static byte[] ReadAllBytes(Stream stream)
        {
            const int BufferSize = 4096;

            var buffer = new byte[BufferSize];
            var result = new List<byte>();

            var bytesRead = 0;
            do
            {
                bytesRead = stream.Read(buffer, 0, BufferSize);
                result.AddRange(buffer.Take(bytesRead));
            } while(bytesRead != 0);

            return result.ToArray();
        }
    }
}
