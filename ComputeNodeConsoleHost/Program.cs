using ComputeNodeServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ComputeNodeConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** Console host ***");
            using (var serviceHost = new ServiceHost(typeof(ComputeNodeService)))
            {
                serviceHost.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press the Enter key to terminate services.");
                Console.ReadLine();
            }
        }
    }
}
