using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerNode.Model
{
    /// <summary>
    /// Класс, описывающий вычислительный узел
    /// </summary>
    public class ComputeNode : Notifier
    {
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // ip-адрес узла
        private string ip;
        public string Ip
        {
            get { return ip; }
            set
            {
                ip = value;
                NotifyPropertyChanged("Ip");
            }
        }

        // url узла
        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                NotifyPropertyChanged("Url");
            }
        }

        // протокол передачи данных
        private Protocol protocol;
        public Protocol Protocol
        {
            get { return protocol; }
            set
            {
                protocol = value;
                NotifyPropertyChanged("Protocol");
            }
        }

        // время подключения
        private DateTime connectionDate;
        public DateTime ConnectionDate
        {
            get { return connectionDate; }
            set
            {
                connectionDate = value;
                NotifyPropertyChanged("ConnectionDate");
            }
        }
    }
}
