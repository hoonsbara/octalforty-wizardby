﻿#region The MIT License
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

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Defines capabilities of a particular <see cref="IDbPlatform"/>.
    /// </summary>
    [Flags()]
    public enum DbPlatformCapabilities : long
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// The <see cref="IDbPlatform"/> supports clustered indexes.
        /// </summary>
        SupportsClusteredIndexes = 1,

        /// <summary>
        /// The <see cref="IDbPlatform"/> supports composite primary keys.
        /// </summary>
        SupportsCompositePrimaryKeys = 2,

        /// <summary>
        /// The <see cref="IDbPlatform"/> supports transactional DDL.
        /// </summary>
        SupportsTransactionalDdl = 2048,
    }
}
