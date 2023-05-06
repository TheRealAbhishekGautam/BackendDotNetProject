using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorials
{
    public class SuperClass
    {
        // This is the normal function and we can't edit this from the child class
        // in order to make it orerwriteable, we have to declare it as virtual class.
        public void abc()
        {
            Console.WriteLine("We are in the super class function");
        }
        public virtual void abc2()
        {
            Console.WriteLine("Orignal Funtion");
        }
    }
}
