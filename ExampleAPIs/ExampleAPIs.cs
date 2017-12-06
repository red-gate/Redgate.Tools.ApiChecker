using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAPIs
{
    public interface IGood
    {
        int GetCount();

        List<int> GetCounts();
    }

    public class SimpleType
    {
        public int A { get; set; }
        public int B { get; set; }
    }

    public class SimpleType2
    {
        public string A { get; set; }
        public string B { get; set; }
        public HashSet<string> C { get; set; }
    }

    public class SimpleType3
    {
        public string A { get; set; }
        public FileInfo B { get; set; }

        internal Newtonsoft.Json.ConstructorHandling C()
        {
            throw new Exception();
        }
    }

    
    public interface ISimpleType4
    {
        HashSet<int> A();
    }

    public class SimpleType5<T>
    {
        public T A() {  throw new Exception(); }
    }

    public class BadType
    {
        public Newtonsoft.Json.ConstructorHandling A() { throw new Exception(); }
    }

    public class BadType2
    {
        public Newtonsoft.Json.ConstructorHandling A { get; set; }
    }

    public class BadType3
    {
        public HashSet<Newtonsoft.Json.ConstructorHandling> A { get; set; }
    }

    public interface IBadType4
    {
        void A(Newtonsoft.Json.ConstructorHandling b);
    }

    public class BadType5
    {
        public class BadType6
        {
            public Newtonsoft.Json.ConstructorHandling D()
            {
                throw new Exception(); 
            }
        }
    }

    public class BadType7<T> where T : Newtonsoft.Json.IJsonLineInfo
    {
    }

    public class BadType8
    {
        public void Swap<T>(T t) where T : Newtonsoft.Json.IJsonLineInfo
        {
        }
    }
}
