using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeNodeServiceLib
{
    public class ComputeNodeService : IComputeNode
    {
        public bool IsWorking() => true;
    }
}
