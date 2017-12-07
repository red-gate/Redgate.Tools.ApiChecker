using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Redgate.Tools.APIChecker
{
    internal class MethodTypes : IGetTypes
    {
        private readonly MethodInfo m_MethodInfo;

        public MethodTypes(MethodInfo methodInfo)
        {
            m_MethodInfo = methodInfo;
        }

        public IEnumerable<Type> Types()
        {
            var genericArguments = m_MethodInfo.GetGenericArguments();
            var constraints = genericArguments.SelectMany(x => x.GetGenericParameterConstraints());

            return m_MethodInfo.GetParameters().Select(x => x.ParameterType).Append(m_MethodInfo.ReturnType)
                .Concat(genericArguments).Concat(constraints).SelectMany(TypeExtensions.ReferencingTypes);
        }

        public string Name => m_MethodInfo.Name;
    }
}