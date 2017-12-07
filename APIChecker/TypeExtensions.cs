using System;
using System.Collections.Generic;

namespace Redgate.Tools.APIChecker
{ 
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> ReferencingTypes(this Type returnType)
        {
            yield return returnType;

            foreach (var arg in returnType.GetGenericArguments())
            {
                foreach (var type in arg.ReferencingTypes())
                    yield return type;
            }
        }
    }
}