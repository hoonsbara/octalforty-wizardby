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

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Basic implementation of <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        #region Private Fields
        private readonly IDictionary<Type, ServiceBuilder> services = new Dictionary<Type, ServiceBuilder>();
        #endregion

        #region IServiceProvider Members
        /// <summary>
        /// Registers service object <paramref name="service"/> within current
        /// service provider.
        /// </summary>
        /// <param name="service"></param>
        public void RegisterService(object service)
        {
            InternalRegisterService(service.GetType(), service);
        }

        /// <summary>
        /// Registers prototype service object which is created using <see cref="serviceBuilder"/>
        /// for every call to <see cref="IServiceProvider.GetService{T}"/> or <see cref="System.IServiceProvider.GetService"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serviceBuilder"></param>
        public void RegisterService(Type type, ServiceBuilder serviceBuilder)
        {
            InternalRegisterService(type, serviceBuilder);
        }

        /// <summary>
        /// Registers prototype service object which is created using <see cref="serviceBuilder"/>
        /// for every call to <see cref="IServiceProvider.GetService{T}"/> or <see cref="System.IServiceProvider.GetService"/>.
        /// </summary>
        /// <param name="serviceBuilder"></param>
        public void RegisterService<T>(ServiceBuilder serviceBuilder)
        {
            RegisterService(typeof(T), serviceBuilder);
        }

        object System.IServiceProvider.GetService(Type serviceType)
        {
            foreach(Type type in services.Keys)
                if(serviceType.IsAssignableFrom(type))
                    return services[type](this);

            return null;
        }

        /// <summary>
        /// Returns the service object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return (T)((System.IServiceProvider)this).GetService(typeof(T));
        }
        #endregion

        private void InternalRegisterService(Type serviceType, object service)
        {
            InternalRegisterService(serviceType, delegate { return service; });
        }

        private void InternalRegisterService(Type serviceType, ServiceBuilder serviceBuilder)
        {
            services[serviceType] = serviceBuilder;
        }
    }
}
