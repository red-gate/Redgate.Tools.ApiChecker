using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Redgate.Tools.APIChecker
{
    /// <summary>
    /// Perform some validation on an assembly and report the results for all exported types to 
    /// a supplied reporter.
    /// 
    /// An exported type is valid if
    /// - It references framework types (determined by belonging to Microsoft)
    /// - It references public types from the same assembly
    /// </summary>
    public class Check
    {
        private readonly IReporter m_Reporter;
        private readonly Assembly m_Assembly;

        public Check(string assemblyName, IReporter reporter)
        {
            m_Reporter = reporter;
            m_Assembly = Assembly.LoadFile(assemblyName);
        }

        public bool ReturnsOnlyOwnedAndSystemTypes(Type t)
        {
            // Check types references by the class
            var typesInClass = new ClassTypes(t);
            var invalidType = CheckForExternalType(t, typesInClass);

            // Process all the exposed methods of the class
            foreach (var method in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(x => new MethodTypes(x)))
            {
                invalidType = invalidType || CheckForExternalType(t, method);
            }

            foreach (var method in t.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(x => new ConstructorTypes(x)))
            {
                invalidType = invalidType || CheckForExternalType(t, method);
            }

            if (invalidType)
            {
                return false;
            }

            m_Reporter.TypeApproved(t);
            return true;
        }

        private bool CheckForExternalType(Type t, IGetTypes type)
        {
            try
            {
                var unownedTypes = type.Types().Where(x => !TypeIsDescribedInAssemblyOrSystemType(x)).ToList();

                if (unownedTypes.Any())
                {
                    m_Reporter.TypeNotApproved(t, type.Name, unownedTypes);
                    return true;
                }
            }
            catch (Exception e)
            {
                m_Reporter.TypeCausedError(t, type.Name, e);
            }
            return false;
        }

        public bool TypeIsDescribedInAssemblyOrSystemType(Type t)
        {
            var typeIsDescribedInAssemblyOrSystemType = m_Assembly.ExportedTypes.Contains(t) || IsMicrosoftType(t) || t.IsGenericParameter;

            return typeIsDescribedInAssemblyOrSystemType;
        }

        // https://stackoverflow.com/questions/962639/detect-if-the-type-of-an-object-is-a-type-defined-by-net-framework/962658
        private bool IsMicrosoftType(Type type)
        {
            var attrs = type.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            return attrs.OfType<AssemblyCompanyAttribute>().Any(attr => attr.Company == "Microsoft Corporation");
        }

        public void Validate()
        {
            foreach (var t in m_Assembly.ExportedTypes)
            {
                ReturnsOnlyOwnedAndSystemTypes(t);
            }
        }
    }
}
