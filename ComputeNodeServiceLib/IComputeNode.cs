using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ComputeNodeServiceLib
{
    [ServiceContract]
    public interface IComputeNode
    {
        [OperationContract]
        bool IsWorking();

        [OperationContract]
        RestorePasswordResult RestorePassword(string passwordHash, char[] startSymbolsRange);
    }
}
