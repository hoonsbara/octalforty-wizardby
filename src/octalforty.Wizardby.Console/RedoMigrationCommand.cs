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
using System.IO;

using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Implements <see cref="MigrationCommand.Redo"/> command logic.
    /// </summary>
    [MigrationCommand(MigrationCommand.Redo)]
    public class RedoMigrationCommand : MigrationCommandBase
    {
        protected override void InternalExecute(MigrationParameters parameters)
        {
            using(StreamReader streamReader = new StreamReader(parameters.MdlFileName, true))
                ServiceProvider.GetService<IMigrationService>().Redo(parameters.ConnectionString, (int)(parameters.VersionOrStep ?? 1), streamReader);
        }
    }
}
