using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class FieldCopy : Attribute
    {
    }

    class Program
    {
        static void Main(string[] args) 
	    {
            MyClass a = new MyClass();
            MyClass b = new MyClass();

            CopyFields<MyClass>(b, a);
	    }

        public static void CopyFields<T>(T dest, T src)
        {
            var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (var sourceField in fields.Where(f => f.GetCustomAttributes(typeof(FieldCopy)).Any()))
            {
                var destProperty = fields.Where(d => d.Name == sourceField.Name).First();
                destProperty.SetValue(dest, sourceField.GetValue(src));
            }
        }
    }

    public class MyClass
    {
        [FieldCopy]
        private int _copyValue;
        private int _nonCopyValue;
    }

}
