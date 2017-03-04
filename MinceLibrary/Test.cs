using Mince.Types;
using Mince;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinceLibrary
{
    // [StaticClass("staticTest")] 
    // Use the [StaticClass(name)] attribute and remove the [Instantiatable(name)] to make
    // a class static. If it were static, in Mince you would type:
    // staticTest.hello();
    // console.print(staticTest.myProperty);
    [Instantiatable("Test")]
    public class Test : MinceObject
    {
        // To use this in Mince just type:
        // myVar = new Test();
        // myVar.hello();
        // console.print(myVar.myPropery);

        private string privateVariable = "hello world";
        
        // The [Exposed] attribute can only be used on properties and methods
        [Exposed]
        public MinceString myProperty
        {
            get { return new MinceString(privateVariable); }
            set { privateVariable = value.ToString(); } // If there is no setter the property will be readonly
        }

        // Constructors can have parameters, but they must be MinceObjects
        public Test()
        {
            CreateMembers();
        }

        // Any accessible methods must have the [Exposed] attribute
        // Any parameters must be MinceObjects
        // At the moment Mince does not support params object[] parameters
        // And does not support multiple methods of the same name, no exceptions.
        [Exposed]
        public MinceNull hello() 
        {
            Console.WriteLine("HELLO!");
            return new MinceNull();
        }
    }
}
