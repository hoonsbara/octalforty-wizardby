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
using System.Data;
using System.IO;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Console
{
    public class FileDbCommandExecutive : IDbExecutive
    {
        private IDbPlatform platform;
        private readonly IDbExecutive innerDbCommandExecutive;
        private readonly TextWriter textWriter;

        public FileDbCommandExecutive(TextWriter textWriter, IDbExecutive innerDbCommandExecutive)
        {
            this.textWriter = textWriter;
            this.innerDbCommandExecutive = innerDbCommandExecutive;
        }

        public IDbPlatform Platform
        {
            get { return platform; }
            set { platform = value; }
        }

        /// <summary>
        /// Executes the given <paramref name="dbOperation"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbOperation"></param>
        /// <returns></returns>
        public T Execute<T>(DbOperation<T> dbOperation)
        {
            //TODO: Is this really no-op?
            return default(T);
        }

        public IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return innerDbCommandExecutive.ExecuteReader(dbCommand);
        }

        public int ExecuteNonQuery(IDbCommand dbCommand)
        {
            textWriter.WriteLine(dbCommand.CommandText);
            return 0;
        }
    }
}
