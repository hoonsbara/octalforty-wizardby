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

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Tests.Core.Db
{
    [TestFixture()]
    public class DbStatementBatchWriterTestFixture
    {
        [Test()]
        public void WriteBatches()
        {
            DbStatementBatchWriter batchWriter = new DbStatementBatchWriter();

            batchWriter.BatchWriter.Write("Batch 1");
            batchWriter.EndBatch();

            batchWriter.BatchWriter.Write("Batch 2");
            batchWriter.EndBatch();

            batchWriter.BatchWriter.Write("Batch 3");

            string[] batches = batchWriter.GetStatementBatches();

            Assert.AreEqual(3, batches.Length);
            Assert.AreEqual("Batch 1", batches[0]);
            Assert.AreEqual("Batch 2", batches[1]);
            Assert.AreEqual("Batch 3", batches[2]);
        }

        [Test()]
        public void WriteBatches2()
        {
            DbStatementBatchWriter batchWriter = new DbStatementBatchWriter();

            batchWriter.BatchWriter.Write("Batch 1");
            batchWriter.EndBatch();

            batchWriter.BatchWriter.Write("Batch 2");
            batchWriter.EndBatch();
            batchWriter.EndBatch();

            batchWriter.BatchWriter.Write("Batch 3");
            batchWriter.EndBatch();
            batchWriter.EndBatch();
            batchWriter.EndBatch();

            string[] batches = batchWriter.GetStatementBatches();

            Assert.AreEqual(3, batches.Length);
            Assert.AreEqual("Batch 1", batches[0]);
            Assert.AreEqual("Batch 2", batches[1]);
            Assert.AreEqual("Batch 3", batches[2]);
        }
    }
}
