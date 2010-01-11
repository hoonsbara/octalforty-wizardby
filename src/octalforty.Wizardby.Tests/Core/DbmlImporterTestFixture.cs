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
