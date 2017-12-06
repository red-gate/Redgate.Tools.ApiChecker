using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;
using Redgate.Tools.APIChecker;

namespace Redgate.Tools.APIChecker.Tests
{
    public class CheckerTests
    {
        [Fact]
        public void FullPathOfAssemblyMustBeSupplied()
        {
            Assert.Throws<ArgumentException>(() => new Check("this name does not exist.dll", Reporter));
        }

        [Fact]
        public void FullPathSuppliedButDoesntExist()
        {
            Assert.Throws<FileNotFoundException>(() => new Check("c:/full/path/this name does not exist.dll", Reporter));
        }

        [Fact]
        public void Test_HarnessWorks()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            Assert.True(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ExampleApis.dll")), "Found " + "ExampleApis.dll" + " in " + currentDirectory);
        }

        [Fact]
        public void Test_Our_Types_Are_Defined_In_Our_Assembly()
        {
            Assert.True(Check.TypeIsDescribedInAssemblyOrSystemType(typeof(ExampleAPIs.SimpleType)), "SimpleType is defined in our assembly");
        }

        [Fact]
        public void These_Types_Are_Ok()
        {
            Assert.True(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.SimpleType)), "All types defined in assembly");
        }

        [Fact]
        public void Returning_A_NewtonSoft_Type_Is_Not_Allowed_In_A_Method()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType)), "Type not used in assembly");
        }

        [Fact]
        public void Returning_A_NewtonSoft_Type_Is_Not_Allowed_In_A_Property()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType2)), "Type not used in assembly");
        }

        [Fact]
        public void Using_Other_Types_Internally_Is_Fine()
        {
            Assert.True(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.SimpleType3)), "Only uses external types internally");
        }

        [Fact]
        public void Using_Types_That_Are_Not_Yours_As_Generic_Arguments_Is_Bad()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType3)), "Only uses external types internally");
        }

        [Fact]
        public void Using_A_Hash_Set_In_An_Interface_Is_Good()
        {
            Assert.True(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.ISimpleType4)), "Using an interface is OK");
        }

        [Fact]
        public void Using_An_External_Type_In_A_Parameter_Is_Not_Allowed()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.IBadType4)), "Using an interface is OK");
        }

        [Fact]
        public void Nested_Bad_Types_Are_Not_Allowed()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType5.BadType6)), "Using an interface is OK");
        }

        [Fact]
        public void Success_Is_Reported()
        {
            Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.ISimpleType4));

            Assert.True(Reporter.ApprovedTypes.Contains(typeof(ExampleAPIs.ISimpleType4)), "Type approved!");
        }

        [Fact]
        public void Failure_Is_Reported()
        {
            Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType));

            var tuple = Reporter.InvalidTypes.Single();

            Assert.Equal(typeof(ExampleAPIs.BadType), tuple.Item1);
            Assert.Equal("A",tuple.Item2.Name);
            Assert.Equal("Newtonsoft.Json.ConstructorHandling",tuple.Item3.Single().FullName);
        }

        [Fact]
        public void Unconstrained_Generics_Are_Fine()
        {
            Assert.True(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.SimpleType5<>)));
        }

        [Fact]
        public void Constrained_Generics_To_External_Types_Are_Bad()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType7<>)));
        }

        [Fact]
        public void Constrained_Generics_To_External_Types_In_Methods_Are_Bad()
        {
            Assert.False(Check.ReturnsOnlyOwnedAndSystemTypes(typeof(ExampleAPIs.BadType8)));
        }

        private class TestReporter : IReporter
        {
            internal readonly List<Type> ApprovedTypes = new List<Type>();

            internal readonly List<Tuple<Type, MethodInfo, IEnumerable<Type>>> InvalidTypes = new 
                List<Tuple<Type, MethodInfo, IEnumerable<Type>>>();

            public void TypeApproved(Type t)
            {
                ApprovedTypes.Add(t);
            }

            public void TypeNotApproved(Type type, MethodInfo method, IEnumerable<Type> unownedTypes)
            {
                InvalidTypes.Add(Tuple.Create(type,method,unownedTypes));
            }

            public void TypeCausedError(Type type, MethodInfo method, Exception exception)
            {
                // TODO Test assembly resolve errors somehow
            }
        }

        private TestReporter Reporter { get; } = new TestReporter();

        // We use this check to find the assembly to avoid problems with shadow-copy
        private Check Check => new Check(typeof(ExampleAPIs.SimpleType).Assembly.Location, Reporter);
    }
}
