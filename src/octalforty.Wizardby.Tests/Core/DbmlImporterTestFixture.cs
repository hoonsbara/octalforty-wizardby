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
using System.Reflection;
using NUnit.Framework;
using octalforty.Wizardby.Core;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core
{
    [TestFixture()]
    public class DbmlImporterTestFixture
    {
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ImportDbmlThrowsArgumentNullExceptionOnNullStream()
        {
            new DbmlImporter().ImportDbml(null);
        }

        [Test()]
        public void ImportDbml()
        {
            var importer = new DbmlImporter();
            Schema schema;
            using(var dbmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Componento.dbml"))
                schema = importer.ImportDbml(dbmlStream);

            Assert.IsNotNull(schema);

            var table = schema.GetTable("dbo", "Component");
            
            Assert.IsNotNull(table);
            Assert.AreEqual("dbo", table.Schema.Name);
            Assert.AreEqual("Component", table.Name);
        }
    }
}
