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
using NUnit.Framework;

using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core.SemanticModel
{
    [TestFixture()]
    public class SchemaTestFixture
    {
        [Test()]
        public void AddTable()
        {
            Schema schema = new Schema();
            TableDefinition table = new TableDefinition("Foo");

            schema.AddTable(table);

            Assert.AreSame(table, schema.Tables[0]);
        }

        [Test()]
        public void GetTable()
        {
            Schema schema = new Schema();
            schema.AddSchema(new SchemaDefinition("dbo"));

            schema.AddTable(new TableDefinition("Foo", schema.Schemas[0]));
            schema.AddTable(new TableDefinition("baR"));

            Assert.IsNull(schema.GetTable("Foo"));

            Assert.AreSame(schema.Tables[0], schema.GetTable("dbo", "Foo"));
            Assert.AreSame(schema.Tables[0], schema.GetTable("DBO", "FOO"));
            Assert.AreSame(schema.Tables[0], schema.GetTable(schema.Schemas[0], "FOO"));

            Assert.AreSame(schema.Tables[1], schema.GetTable("BaR"));
            Assert.AreSame(schema.Tables[1], schema.GetTable("baR"));

            Assert.IsNull(schema.GetTable("Qux"));
        }
    }
}
