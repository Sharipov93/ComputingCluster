using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerNode.Model
{
    public class Protocol : Notifier
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

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string protValue;
        public string ProtValue
        {
            get { return protValue; }
            set
            {
                protValue = value;
                NotifyPropertyChanged("ProtValue");
            }
        }

        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                NotifyPropertyChanged("Port");
            }
        }
    }
}
