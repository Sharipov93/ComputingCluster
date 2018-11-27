using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerNode.Model
{
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
