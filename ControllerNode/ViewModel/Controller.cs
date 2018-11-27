using ControllerNode.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerNode.ViewModel
{
    public class Controller : Notifier
    {
        // конструктор класса контроллер
        public Controller()
        {
            SetButtonsEndbled(true);
            SetInformationLog();
            SetNodesInformationLog();
        }


        // свойства видимости кнопок контроллера
        private bool addNodeBtnEnabled;
        public bool AddNodeBtnEnabled
        {
            get { return addNodeBtnEnabled; }
            set
            {
                addNodeBtnEnabled = value;
                NotifyPropertyChanged("AddNodeBtnEnabled");
            }
        }

        private bool removeNodeBtnEnabled;
        public bool RemoveNodeBtnEnabled
        {
            get { return removeNodeBtnEnabled; }
            set
            {
                removeNodeBtnEnabled = value;
                NotifyPropertyChanged("RemoveNodeBtnEnabled");
            }
        }

        private bool startComputeBtnEnabled;
        public bool StartComputeBtnEnabled
        {
            get { return startComputeBtnEnabled; }
            set
            {
                startComputeBtnEnabled = value;
                NotifyPropertyChanged("StartComputeBtnEnabled");
            }
        }

        private bool cancelComputeBtnEnabled;
        public bool CancelComputeBtnEnabled
        {
            get { return cancelComputeBtnEnabled; }
            set
            {
                cancelComputeBtnEnabled = value;
                NotifyPropertyChanged("CancelComputeBtnEnabled");
            }
        }

        // логи результата
        private string useInformationLog;
        public string UseInformationLog
        {
            get { return useInformationLog; }
            set
            {
                useInformationLog = value;
                NotifyPropertyChanged("UseInformationLog");
            }
        }

        private string nodesInformationLog;
        public string NodesInformationLog
        {
            get { return nodesInformationLog; }
            set
            {
                nodesInformationLog = value;
                NotifyPropertyChanged("NodesInformationLog");
            }
        }

        private string resultInformationlog;
        public string ResultInformationlog
        {
            get { return resultInformationlog; }
            set
            {
                resultInformationlog = value;
                NotifyPropertyChanged("ResultInformationlog");
            }
        }

        private string logText;
        public string LogText
        {
            get { return logText; }
            set
            {
                logText = value;
                NotifyPropertyChanged("LogText");
            }
        }

        // настройка enabled кнопок контролера
        private void SetButtonsEndbled(bool addNodeBtn = false, bool removeNodeBtn = false, 
            bool startComputeBtn = false, bool cancelComputeBtn = false)
        {
            AddNodeBtnEnabled = addNodeBtn;
            RemoveNodeBtnEnabled = removeNodeBtn;
            StartComputeBtnEnabled = startComputeBtn;
            CancelComputeBtnEnabled = cancelComputeBtn;
        }

        public ObservableCollection<ComputeNode> ComputeNodes { get; set; }

        // вывод результата работы и др. информации
        private void SetInformationLog()
        {
            string instruction = "1. Запустите службу Windows (или консольное приложение) на необходимых компьютерах.";
            instruction += "\n2. Для подключения к сервису, добавьте 1(+) вычислительных узлов.";
            instruction += "\n   (При добавлении узла, укажите ip-адрес компьютера, где выполняется служба(консольное приложение) и протокол передачи данных(http, tcp))";
            instruction += "\n3. Введите пароль. (Допустимые символы: { [A - Z], [a - z], [0 - 9] })";
            instruction += "\n4. Нажмите кнопку 'Запустить'";

            UseInformationLog = instruction;
        }

        private void SetNodesInformationLog()
        {
            string nodesInfo = "";
            if (ComputeNodes == null || ComputeNodes.Count <= 0)
                nodesInfo = "(!) Подключенных вычислительных узлов нет.";

            NodesInformationLog = nodesInfo;
        }

        private void SetResultInformationlog()
        {

        }
    }
}
