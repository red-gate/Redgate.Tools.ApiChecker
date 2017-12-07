using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Redgate.Tools.APIChecker
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Please provide the full path to a .NET assembly to validate");
                return;
            }

            var check = new Check(args[0], new ConsoleReporter());
            check.Validate();
        }

        private class ConsoleReporter : IReporter
        {
            public void TypeApproved(Type t)
            {
                Console.WriteLine("SUCCESS: {0} is a valid type", t.FullName);
            }

            public void TypeNotApproved(Type type, string name, IEnumerable<Type> unownedTypes)
            {
                Console.WriteLine("WARNING: {0}.{1} uses {2} external to the assembly", type.Name, name, string.Join(",", unownedTypes.Select(x => x.Name)));
            }

            public void TypeCausedError(Type type, string name, Exception exception)
            {
                Console.WriteLine("ERROR: Unable to check {0}.{1} because of {2}", type.Name, name, exception.Message);
            }
        }
    }
}
