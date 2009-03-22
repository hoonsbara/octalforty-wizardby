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
using System.Collections.Specialized;
using System.IO;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Deployment.Impl;

namespace octalforty.Wizardby.Core.Deployment
{
    public class DeploymentInfoParser
    {
        public IDeploymentInfo ParseDeploymentInfo(StreamReader streamReader)
        {
            MdlScanner scanner = new MdlScanner(new SourceReader(streamReader));
            scanner.RegisterKeyword("deployment");
            scanner.RegisterKeyword("environment");

            IMdlParser mdlParser = new MdlParser(scanner);

            IDeploymentNode deploymentNode = (IDeploymentNode)mdlParser.Parse();
            EnvironmentCollection environments = new EnvironmentCollection();

            foreach(IEnvironmentNode environmentNode in deploymentNode.ChildNodes)
            {
                environments.Add(ParseEnvironment(environmentNode));
            } // foreach

            return new DeploymentInfo(environments);
        }

        private static IEnvironment ParseEnvironment(IEnvironmentNode environmentNode)
        {
            NameValueCollection properties = new NameValueCollection();
            foreach(IAstNodeProperty astNodeProperty in environmentNode.Properties)
                properties.Add(astNodeProperty.Name, astNodeProperty.Value.ToString());

            return new Impl.Environment(environmentNode.Name, properties);
        }
    }
}
