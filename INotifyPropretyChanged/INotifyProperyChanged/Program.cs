using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INotifyProperyChangedDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var object1 = new ExampleObject<ValueToUpperCaseFormatter>();
            object1.PropertyChanged += (sender, arg) => {
                Console.WriteLine(arg); };
            object1.Data = "changed";
        }
    }
}
