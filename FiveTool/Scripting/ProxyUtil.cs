using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace FiveTool.Scripting
{
    internal static class ProxyUtil
    {
        /// <summary>
        /// Converts a UserData value to a proxy object if applicable.
        /// </summary>
        /// <param name="data">The UserData value to convert.</param>
        /// <returns>The proxy object if a conversion was possible, otherwise the original value.</returns>
        public static object GetUserDataAsProxy(UserData data)
        {
            if (data.Object == null)
                return null;
            var proxyDescriptor = data.Descriptor as ProxyUserDataDescriptor;
            return proxyDescriptor != null ? ToProxyObject(data.Object, proxyDescriptor) : data.Object;
        }

        private static object ToProxyObject(object obj, ProxyUserDataDescriptor descriptor)
        {
            // HACK: Use reflection to get the internal proxy factory
            var field = typeof(ProxyUserDataDescriptor).GetField("m_ProxyFactory",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var factory = (IProxyFactory)field.GetValue(descriptor);
            return factory.CreateProxyObject(obj);
        }
    }
}
