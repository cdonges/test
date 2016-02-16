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

            a.SetValues(7, 10);

            CopyFields<MyClass>(b, a);
	    }

        /// <summary>
        /// Copy all fields from src to dest where they have a FieldCopy Attribute
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="dest">destination object</param>
        /// <param name="src">source object</param>
        public static void CopyFields<T>(T dest, T src)
        {
            // get fields using reflection
            var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            // find fields that have the FieldCopy attribute
            foreach (var sourceField in fields.Where(f => f.GetCustomAttributes(typeof(FieldCopy)).Any()))
            {
                // set value in destination object
                sourceField.SetValue(dest, sourceField.GetValue(src));
            }
        }
    }

    public class MyClass
    {
        [FieldCopy]
        private int _copyValue;
        private int _nonCopyValue;

        public void SetValues(int copyValue, int nonCopyValue)
        {
            this._copyValue = copyValue;
            this._nonCopyValue = nonCopyValue;
        }
    }

}
