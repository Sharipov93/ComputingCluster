using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeNodeServiceLib
{
    // класс, который возвращается как результат работы службы
    public class RestorePasswordResult
    {
        // флаг, показывающий найден ли пароль
        public bool PasswordIsRestored { get; set; }
        
        // начальный хэш пароля
        public string PasswordHash { get; set; }

        // подобранный пароль
        public string RestorePassword { get; set; }

        // количество операций, сделанных сервисом по подбору пароля
        public long OperationCount { get; set; }

        // время затраченное сервисом на подбор пароля
        public int OperationsLeadTime { get; set; }
    }
}
