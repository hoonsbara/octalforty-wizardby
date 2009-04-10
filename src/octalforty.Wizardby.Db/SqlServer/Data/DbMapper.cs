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
using System.Data;
using System.Diagnostics;

namespace octalforty.Wizardby.Db.SqlServer.Data
{
    public class Mapping
    {
        private readonly string source;
        private readonly string target;
        private readonly bool nullable;

        public string Source
        {
            [DebuggerStepThrough]
            get { return source; }
        }

        public string Target
        {
            [DebuggerStepThrough]
            get { return target; }
        }

        public bool Nullable
        {
            [DebuggerStepThrough]
            get { return nullable; }
        }

        public Mapping(string source, string target)
        {
            this.source = source;
            this.target = target;
        }

        public Mapping(string source, string target, bool nullable)
        {
            this.source = source;
            this.target = target;
            this.nullable = nullable;
        }
    }

    public class MappingSource
    {
        private object source;

        public MappingSource(object source)
        {
            this.source = source;
        }
    }

    public class ObjectMapper
    {
        private readonly IList<Mapping> mappings = new List<Mapping>();

        public void AddMapping(Mapping mapping)
        {
            mappings.Add(mapping);
        }

        public void Map(object source, object target)
        {
            IPropertyAccessor propertyAccessor = GetPropertyAccessorFor(source);
            IPropertyAccessor mappingTarget = GetPropertyAccessorFor(target);

            foreach(Mapping mapping in mappings)
            {
                Map(propertyAccessor, mappingTarget, mapping);
            } // foreach
        }

        private IPropertyAccessor GetPropertyAccessorFor(object o)
        {
            if(o is IDataRecord)
                return new DataRecordPropertyAccessor((IDataRecord)o);

            return new ObjectPropertyAccessor();
        }

        private void Map(IPropertyAccessor source, IPropertyAccessor target, Mapping mapping)
        {
            if(source[mapping.Source] == null && !mapping.Nullable)
                return;

            target[mapping.Target] = source[mapping.Source];
        }
    }

    public interface IPropertyAccessor
    {
        object this[string name]
        { get; set; }
    }

    public class ObjectPropertyAccessor : IPropertyAccessor
    {

        public object this[string name]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }

    public class DataRecordPropertyAccessor : IPropertyAccessor
    {
        private readonly IDataRecord dataRecord;

        public DataRecordPropertyAccessor(IDataRecord dataRecord)
        {
            this.dataRecord = dataRecord;
        }

        public object this[string name]
        {
            get { return dataRecord[name]; }
            set { throw new NotSupportedException(); }
        }
    }
}
