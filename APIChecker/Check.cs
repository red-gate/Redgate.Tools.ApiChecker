﻿using System;
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
            var publicMethods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var invalidMethods = false;

            foreach(var method in publicMethods)
            {
                try
                {
                    var unownedTypes = TypesInMethod(method).Where(x => !TypeIsDescribedInAssemblyOrSystemType(x))
                        .ToList();

                    if (unownedTypes.Any())
                    {
                        m_Reporter.TypeNotApproved(t, method, unownedTypes);
                        invalidMethods = true;
                    }
                }
                catch (Exception e)
                {
                    m_Reporter.TypeCausedError(t, method, e);
                }
            }

            if (invalidMethods)
            {
                return false;
            }

            m_Reporter.TypeApproved(t);
            return true;
        }

        private IEnumerable<Type> TypesInMethod(MethodInfo method)
        {
            return method.GetParameters().Select(x => x.ParameterType).Append(method.ReturnType).SelectMany(Types);
        }

        private static IEnumerable<Type> Types(Type returnType)
        {
            yield return returnType;

            foreach (var arg in returnType.GetGenericArguments())
            {
                yield return arg;
            }
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