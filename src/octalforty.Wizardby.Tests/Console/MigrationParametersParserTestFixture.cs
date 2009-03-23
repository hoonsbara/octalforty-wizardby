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
using octalforty.Wizardby.Console;

namespace octalforty.Wizardby.Tests.Console
{
    [TestFixture()]
    public class MigrationParametersParserTestFixture
    {
        [Test()]
        public void ParseMigrationParameters()
        {
            MigrationParametersParser parametersParser = new MigrationParametersParser();
            MigrationParameters parameters = parametersParser.ParseMigrationParameters(new string[]
                {
                    "downgrade", 
                    "/c:\"data source=GOGOLEV_A\\SQLEXPRESS;initial catalog=test;integrated security=SSPI;\"", 
                    "/p:sqlserver",
                    "/o:downgrade.sql"
                });

            Assert.AreEqual(MigrationCommand.Downgrade, parameters.Command);
            Assert.IsNull(parameters.VersionOrStep);
            Assert.AreEqual("data source=GOGOLEV_A\\SQLEXPRESS;initial catalog=test;integrated security=SSPI;", parameters.ConnectionString);
            Assert.AreEqual("sqlserver", parameters.PlatformAlias);
            Assert.AreEqual("downgrade.sql", parameters.OutputFileName);

            parameters = parametersParser.ParseMigrationParameters(new string[]
                {
                    "migrate",
                    "20090226100609",
                    "/connection:\"data source=GOGOLEV_A\\SQLEXPRESS;initial catalog=test;integrated security=SSPI;\"", 
                    "/PLATFORM:sqlserver",
                    "/e:dev"
                });

            Assert.AreEqual(MigrationCommand.Migrate, parameters.Command);
            Assert.AreEqual(20090226100609, parameters.VersionOrStep.Value);
            Assert.AreEqual("data source=GOGOLEV_A\\SQLEXPRESS;initial catalog=test;integrated security=SSPI;", parameters.ConnectionString);
            Assert.AreEqual("sqlserver", parameters.PlatformAlias);
            Assert.AreEqual("dev", parameters.Environment);
            Assert.IsNull(parameters.OutputFileName);
        }

        [Test()]
        public void ParseMigrationParameters2()
        {
            MigrationParametersParser parametersParser = new MigrationParametersParser();
            MigrationParameters parameters = parametersParser.ParseMigrationParameters(new string[]
                {
                    "downgrade"
                });

            Assert.AreEqual(MigrationCommand.Downgrade, parameters.Command);
        }

        [Test()]
        public void ParseMigrationParameters3()
        {
            MigrationParametersParser parametersParser = new MigrationParametersParser();
            MigrationParameters parameters = parametersParser.ParseMigrationParameters(new string[]
                {
                    "d"
                });

            Assert.AreEqual(MigrationCommand.Downgrade, parameters.Command);
        }

        [Test()]
        public void ParseMigrationParameters4()
        {
            MigrationParametersParser parametersParser = new MigrationParametersParser();
            MigrationParameters parameters = parametersParser.ParseMigrationParameters(new string[]
                {
                    "re"
                });

            Assert.AreEqual(MigrationCommand.Redo, parameters.Command);
        }
    }
}
