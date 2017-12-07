using System;
using System.Collections.Generic;

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
        /// The type is not approved because name references unownedTypes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="unownedTypes"></param>
        void TypeNotApproved(Type type, string name, IEnumerable<Type> unownedTypes);

        /// <summary>
        /// An exception occured processed name on type because of exception
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="exception"></param>
        void TypeCausedError(Type type, string name, Exception exception);
    }
}