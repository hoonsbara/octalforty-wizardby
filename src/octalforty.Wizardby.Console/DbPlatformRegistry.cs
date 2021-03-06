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

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Console
{
    public class DbPlatformRegistry : AttributeAwareTypeRegistry<DbPlatformAttribute>
    {
        #region Private Fields
        private readonly IDictionary<string, IDbPlatform> platforms = new Dictionary<string, IDbPlatform>();
        #endregion

        public void RegisterPlatform<TDbPlatform>()
            where TDbPlatform : IDbPlatform, new()
        {
            RegisterPlatform(typeof(TDbPlatform), new TDbPlatform());
        }

        public IDbPlatform ResolvePlatform(string alias)
        {
            if(alias == null) 
                throw new ArgumentNullException("alias");

            return platforms.ContainsKey(alias) ?
                platforms[alias] :
                null;
        }

        public string GetPlatformName(IDbPlatform dbPlatform)
        {
            if(dbPlatform == null) 
                throw new ArgumentNullException("dbPlatform");

            DbPlatformAttribute dbPlatformAttribute =
                (DbPlatformAttribute)Attribute.GetCustomAttribute(dbPlatform.GetType(), typeof(DbPlatformAttribute));
            return dbPlatformAttribute.Name;
        }

        private void RegisterPlatform(Type platformType, IDbPlatform platform)
        {
            DbPlatformAttribute dbPlatformAttribute =
                (DbPlatformAttribute)Attribute.GetCustomAttribute(platformType, typeof(DbPlatformAttribute));
            RegisterPlatform(platform, dbPlatformAttribute);
        }

        private void RegisterPlatform(IDbPlatform platform, DbPlatformAttribute dbPlatformAttribute)
        {
            platforms[dbPlatformAttribute.Alias] = platform;
        }

        #region AttributeAwareTypeRegistry<DbPlatformAttribute> Memberss
        protected override void RegisterType(Type type, DbPlatformAttribute attribute)
        {
            RegisterPlatform((IDbPlatform)Activator.CreateInstance(type), attribute);
        }
        #endregion
    }
}