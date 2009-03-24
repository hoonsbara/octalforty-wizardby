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
using System.Diagnostics;
using System.Text;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Provides an abstract base class for <see cref="IDbConnectionStringBuilder"/> implementations.
    /// </summary>
    public abstract class DbConnectionStringBuilderBase : DbPlatformDependencyBase, IDbConnectionStringBuilder
    {
        #region Private Fields
        private bool ignoreUnmappedKeys;
        private readonly StringBuilder connectionStringBuilder = new StringBuilder();
        private readonly IDictionary<string, string> keyMappings = new Dictionary<string, string>();
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets a reference to the <see cref="StringBuilder"/> used to construct a connection string.
        /// </summary>
        protected StringBuilder ConnectionStringBuilder
        {
            get { return connectionStringBuilder; }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets a flag which indicates whether to ignore unmapped keys while
        /// building the connection string. 
        /// </summary>
        public bool IgnoreUnmappedKeys
        {
            [DebuggerStepThrough]
            get { return ignoreUnmappedKeys; }
            [DebuggerStepThrough]
            set { ignoreUnmappedKeys = value; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="DbConnectionStringBuilderBase"/> class.
        /// </summary>
        protected DbConnectionStringBuilderBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DbConnectionStringBuilderBase"/> class.
        /// </summary>
        /// <param name="ignoreUnmappedKeys"></param>
        protected DbConnectionStringBuilderBase(bool ignoreUnmappedKeys)
        {
            this.ignoreUnmappedKeys = ignoreUnmappedKeys;
        }

        /// <summary>
        /// Registers key mapping from <paramref name="key"/> to <paramref name="mappedKey"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mappedKey"></param>
        public void RegisterKeyMapping(string key, string mappedKey)
        {
            keyMappings[GetKeyMap(key)] = mappedKey;
        }

        #region IDbConnectionStringBuilder Members
        /// <summary>
        /// Appends a <paramref name="key"/>-<paramref name="value"/> pair to the connection string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void AppendKeyValuePair(string key, string value)
        {
            if(!IsKeyMapped(key) && IgnoreUnmappedKeys)
                return;

            ConnectionStringBuilder.AppendFormat("{0}={1};", 
                IsKeyMapped(key) ?
                    GetMappedKey(key) :
                    key, value);
        }

        /// <summary>
        /// Returns the connection string associated with this <see cref="IDbConnectionStringBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ConnectionStringBuilder.ToString();
        }
        #endregion

        #region Overridables
        protected virtual bool IsKeyMapped(string key)
        {
            return keyMappings.ContainsKey(GetKeyMap(key));
        }

        protected virtual string GetMappedKey(string key)
        {
            if(!IsKeyMapped(key))
                throw new InvalidOperationException();

            return keyMappings[GetKeyMap(key)];
        }
        #endregion

        private static string GetKeyMap(string key)
        {
            return key.ToUpperInvariant();
        }
    }
}
