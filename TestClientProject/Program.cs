using ComputeNodeServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace TestClientProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Binding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress("http://192.168.0.107:8090/ComputeNodeServiceLib.ComputeNodeService");

            IComputeNode proxy = ChannelFactory<IComputeNode>.CreateChannel(binding, address);
            using (proxy as IDisposable)
            {
                try
                {
                    bool result = proxy.IsWorking();
                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.WriteLine(e.InnerException?.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
