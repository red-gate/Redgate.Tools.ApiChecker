using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Redgate.Tools.APIChecker
{
    internal class ConstructorTypes : IGetTypes
    {
        private readonly ConstructorInfo m_ConstructorInfo;

        public ConstructorTypes(ConstructorInfo constructorInfo)
        {
            m_ConstructorInfo = constructorInfo;
        }

        public IEnumerable<Type> Types()
        {
            return m_ConstructorInfo.GetParameters().Select(x => x.ParameterType).SelectMany(TypeExtensions.ReferencingTypes);
        }

        public string Name => m_ConstructorInfo.Name;
    }
}