using System;
using System.Collections.Generic;

namespace Redgate.Tools.APIChecker
{
    internal interface IGetTypes
    {
        IEnumerable<Type> Types();

        string Name { get; }
    }
}
