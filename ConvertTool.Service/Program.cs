using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertTool.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            MyOA.URIResource.Node.Run(System.Windows.Forms.Application.StartupPath.TrimEnd('/', '\\'), args);

            Console.WriteLine("Press Any Key ...");
            Console.ReadKey();
        }
    }
}
