using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeNodeServiceLib
{
    public class RestorePasswordResult
    {
        public bool PasswordIsRestored { get; set; }
        public string PasswordHash { get; set; }
        public string RestorePassword { get; set; }
        public long OperationCount { get; set; }
        public int OperationsLeadTime { get; set; }
    }
}
