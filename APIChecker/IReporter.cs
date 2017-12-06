using System;
using System.Collections.Generic;
using System.Reflection;

namespace Redgate.Tools.APIChecker
{
    public interface IReporter
    {
        /// <summary>
        /// The type t only contains methods/properties exporting types the assembly owns
        /// </summary>
        /// <param name="t"></param>
        void TypeApproved(Type t);
        /// <summary>
        /// The type is not approved because method references unownedTypes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="unownedTypes"></param>
        void TypeNotApproved(Type type, MethodInfo method, IEnumerable<Type> unownedTypes);
        /// <summary>
        /// An exception occured processed method on type because of exception
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="exception"></param>
        void TypeCausedError(Type type, MethodInfo method, Exception exception);
    }
}