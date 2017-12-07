using System;
using System.Collections.Generic;
using System.Linq;

namespace Redgate.Tools.APIChecker
{
    internal class ClassTypes : IGetTypes
    {
        private Type m_Class;

        public ClassTypes(Type @class)
        {
            m_Class = @class;
        }

        public IEnumerable<Type> Types()
        {
            return m_Class.GetGenericArguments().SelectMany(x => x.GetGenericParameterConstraints()).Append(m_Class);
        }

        public string Name => m_Class.Name;
    }
}