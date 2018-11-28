using ComputeNodeServiceLib;
using ControllerNode.Model;
using ControllerNode.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ControllerNode.ViewModel
{
    public class Controller : Notifier
    {
        // конструктор класса контроллер
        public Controller()
        {
            SetButtonsEndbled();
            SetInformationLog();
            SetNodesInformationLog();
            
            LoadProtocols();
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

        private string nodeComputeOutput;
        public string NodeComputeOutput
        {
            get { return nodeComputeOutput; }
            set
            {
                nodeComputeOutput = value;
                NotifyPropertyChanged("NodeComputeOutput");
            }
        }

        // хэш-код пароля
        private string hash;
        public string Hash
        {
            get { return hash; }
            set
            {
                hash = value;
                NotifyPropertyChanged("Hash");
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    Hash = "";

                password = value;
                NotifyPropertyChanged("Password");
            }
        }

        // ip - адрес
        private string firstIpPart;
        public string FirstIpPart
        {
            get { return firstIpPart; }
            set
            {
                int intVal = Int32.Parse(value);
                if (intVal > 255) value = "255";

                firstIpPart = value;
                NotifyPropertyChanged("FirstIpPart");
            }
        }

        private string secondIpPart;
        public string SecondIpPart
        {
            get { return secondIpPart; }
            set
            {
                int intVal = Int32.Parse(value);
                if (intVal > 255) value = "255";


                secondIpPart = value;
                NotifyPropertyChanged("SecondIpPart");
            }
        }

        private string thirdIpPart;
        public string ThirdIpPart
        {
            get { return thirdIpPart; }
            set
            {
                int intVal = Int32.Parse(value);
                if (intVal > 255) value = "255";


                thirdIpPart = value;
                NotifyPropertyChanged("ThirdIpPart");
            }
        }

        private string fourthIpPart;
        public string FourthIpPart
        {
            get { return fourthIpPart; }
            set
            {
                int intVal = Int32.Parse(value);
                if (intVal > 255) value = "255";


                fourthIpPart = value;
                NotifyPropertyChanged("FourthIpPart");
            }
        }


        // коллекция протоколов передачи данных и их загрузка
        public ObservableCollection<Protocol> Protocols { get; set; }
        private Protocol selectedProtocol;
        public Protocol SelectedProtocol
        {
            get { return selectedProtocol; }
            set
            {
                selectedProtocol = value;
                NotifyPropertyChanged("SelectedProtocol");
            }
        }

        private void LoadProtocols()
        {
            Protocols = new ObservableCollection<Protocol>
            {
                new Protocol { Id = 0, Name = "HTTP", ProtValue = "http://", Port = 8090 },
                new Protocol { Id = 1, Name = "TCP", ProtValue = "net.tcp://", Port = 9191 }
            };

            SelectedProtocol = Protocols.First();
        }

        // коллекция для хранения зарегестрированных узлов
        public ObservableCollection<ComputeNode> ComputeNodes { get; set; } = new ObservableCollection<ComputeNode>();
        private ComputeNode selectedComputeNode;
        public ComputeNode SelectedComputeNode
        {
            get { return selectedComputeNode; }
            set
            {
                selectedComputeNode = value;
                NotifyPropertyChanged("SelectedComputeNode");
            }
        }
        
        // команда валидации ввода ip - адреса
        private ControllerCommand numbValidateCommand;
        public ControllerCommand NumbValidateCommand
        {
            get
            {
                return numbValidateCommand ??
                    (numbValidateCommand = new ControllerCommand(obj =>
                    {
                        var validateTxtBox = obj as System.Windows.Controls.TextBox;
                        if (validateTxtBox == null) return;

                        string currentText = validateTxtBox.Text;
                        if (string.IsNullOrEmpty(currentText)) return;

                        char[] symbols = currentText.ToCharArray();
                        foreach (var s in symbols)
                            if (!char.IsDigit(s))
                                validateTxtBox.Text = currentText.Remove(currentText.IndexOf(s), 1);
                    }));
            }
        }

        // команда запуска узлов для восстановления пароля 
        private ControllerCommand restorePasswordCommand;
        public ControllerCommand RestorePasswordCommand
        {
            get
            {
                return restorePasswordCommand ??
                    (restorePasswordCommand = new ControllerCommand(obj =>
                    {
                        if (ComputeNodes == null || ComputeNodes.Count <= 0)
                        {
                            new InformationMessageBox("Добавьте вычислительный узел").ShowDialog();
                            return;
                        }
                        if (string.IsNullOrEmpty(Password))
                        {
                            new InformationMessageBox("Пожалуйста, введите пароль").ShowDialog();
                            return;
                        }
                        if (!isValidPasswordSymbols())
                        {
                            new InformationMessageBox($"Допустимые символы: [0 - 9], [A - Z], [a - z]").ShowDialog();
                            return;
                        }

                        Hash = BitConverter.ToString(GenerateHashPassword(Password))?.Replace("-", "")?.ToLower();

                        SetResultInformationlog($"Сгенерирован хэш-код для введеного пароля [{Password}]: {Hash}");
                        SetButtonsEndbled(true);

                        var task = Task.Factory.StartNew(RestorePassword);
                    }));
            }
        }

        // команда пользовательской отмены восстановленя пароля
        private ControllerCommand cancelPasswordCommand;
        public ControllerCommand CancelPasswordCommand
        {
            get
            {
                return cancelPasswordCommand ??
                    (cancelPasswordCommand = new ControllerCommand(obj =>
                    {
                        var window = new RemoveMsgBox("Остановить восстановления пароля?");
                        if (!(bool)window.ShowDialog()) return;

                        SetResultInformationlog("(!) ПОЛЬЗОВАТЕЛЬ ОТМЕНИЛ ВЫПОЛНЕНИЕ (Завершилось выполнение всех узлов)");

                        cancelCompute();
                    }));
            }
        }

        private void cancelCompute()
        {
            SetButtonsEndbled();

            if (timer != null)
                timer.Change(Timeout.Infinite, 0);

            if (channels == null || channels.Count <= 0) return;
            foreach (var channel in channels)
            {
                if (channel.State != CommunicationState.Closed)
                {
                    try
                    {
                        channel.Abort();
                    }
                    catch { }
                }
            }

            channels = null;
        }

        private byte[] GenerateHashPassword(string password)
        {
            var utf8 = Encoding.UTF8;
            var data = utf8.GetBytes(password);
            byte[] passwordHash = null;
            try
            {
                using (var alg = new SHA256Managed())
                {
                    passwordHash = alg.ComputeHash(data);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при хешировании пароля.\n{e.Message}");
            }

            return passwordHash;
        }

        // строка допустимых символов в пароле. При передаче серверу будет преобразованна в char[]
        private string allowSymbols = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static int startIndex;
        private static int startSymbolsCount;

        /* таймер для вывода общего времени всего процесса восстановления пароля 1 - n узлами
           и свойство для отображения этого времени
        */
        private Timer timer;

        private int allProcessTime;
        public int AllProcessTime
        {
            get { return allProcessTime; }
            set
            {
                allProcessTime = value;
                NotifyPropertyChanged("AllProcessTime");
            }
        }

        // каналы данных
        private IList<ComputeNodeService.ComputeNodeClient> channels;


        private int allTime;
        public int AllNodeTime
        {
            get { return allTime; }
            set
            {
                allTime = value;
                NotifyPropertyChanged("AllNodeTime");
            }
        }

        private long countOper;
        public long AllNodeKeys
        {
            get { return countOper; }
            set
            {
                countOper = value;
                NotifyPropertyChanged("AllNodeKeys");
            }
        }

        private void RestorePassword()
        {
            AllNodeTime = 0;
            AllNodeKeys = 0;

            AllProcessTime = 0;

            timer = new Timer(obj =>
            {
                AllProcessTime++;
            }, null, 0, 1000);

            startIndex = 0;
            startSymbolsCount = allowSymbols.Length / ComputeNodes.Count;

            channels = new List<ComputeNodeService.ComputeNodeClient>();
            foreach (var node in ComputeNodes)
            {
                //Task.Factory.StartNew(() =>
                //{
                    Binding binding = null;
                    if (node.Protocol.Id == 0)
                        binding = new BasicHttpBinding();
                    else
                        binding = new NetTcpBinding();

                    var address = new EndpointAddress(node.Url);

                    var proxy = new ComputeNodeService.ComputeNodeClient(binding, address);
                    channels.Add(proxy);

                    string startSymbols = allowSymbols.Substring(startIndex, startSymbolsCount);
                    startIndex += startSymbolsCount;
                    SetResultInformationlog($"Отправлен запрос на вычисление на узел {node.Url}: \n - Стартовый массив символов: [{startSymbols}]");
                    
                    try
                    {
                        IAsyncResult result = proxy.BeginRestorePassword(Hash, startSymbols.ToCharArray(),
                            ar =>
                            {
                                //Task.Factory.StartNew(() =>
                                //{
                                    try
                                    {
                                        var computeResult = proxy.EndRestorePassword(ar);
                                        SetResultInformationlog($"(!) Узел {node.Url} завершил работу: {DateTime.Now}");
                                        SetNodesComputeOutputLog(computeResult, node.Url);
                                        SetButtonsEndbled();
                                        timer.Change(Timeout.Infinite, 0);

                                        if (computeResult.PasswordIsRestored)
                                        {
                                            foreach (var c in channels)
                                            {
                                                if (c.State != CommunicationState.Closed)
                                                {
                                                    var r = c.BeginStopPasswordComputing(cr =>
                                                    {
                                                        //var comRes = c.EndStopPasswordComputing(cr);
                                                        //AllNodeTime += comRes.OperationsLeadTime;
                                                        //AllNodeKeys += comRes.OperationCount;
                                                    }, null);
                                                }
                                            }

                                            MessageBox.Show(computeResult.RestorePassword);
                                            //cancelCompute();
                                            //MessageBox.Show($"time: {allTime}... operation: {countOper}");
                                        }
                                    }
                                    catch
                                    {

                                    }
                                //});
                            }, null);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                //});
            }
        }
        
        
        // комманда добавления нового узла
        private ControllerCommand addNodeCommand;
        public ControllerCommand AddNodeCommand
        {
            get
            {
                return addNodeCommand ??
                    (addNodeCommand = new ControllerCommand(obj =>
                    {
                        var nodeWindow = new ComputeNodeWindow(this);
                        if (!(bool)nodeWindow.ShowDialog())
                            return;

                        //string ip = $"{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}";
                        //bool existNode = ComputeNodes != null && ComputeNodes.Where(c => 
                        //    c.Ip.Contains(ip)).Count() > 0 ? true : false;

                        //if (existNode)
                        //{
                        //    new InformationMessageBox($"Приложение уже содержит узел: {ip}").ShowDialog();
                        //    return;
                        //}

                        bool connect = false;
                        try
                        {
                            connect = CheckConnection();
                        }
                        catch
                        {
                            connect = false;
                        }

                        if (!connect) return;

                        AddNewComputeNode();
                        SetButtonsEndbled();
                        SetNodesInformationLog();
                    }));
            }
        }
        
        // Проверка вводимых символов пароля
        private bool isValidPasswordSymbols()
        {
            bool isValid = true;
            foreach (char c in Password)
            {
                if (char.IsDigit(c) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122))
                    continue;

                isValid = false;
            }

            return isValid;
        }

        // команда удаления узла
        private ControllerCommand removeNodeCommand;
        public ControllerCommand RemoveNodeCommand
        {
            get
            {
                return removeNodeCommand ??
                    (removeNodeCommand = new ControllerCommand(obj =>
                    {
                        if (SelectedComputeNode == null)
                        {
                            new InformationMessageBox("Пожалуйста, выберите узел для удаления").ShowDialog();
                            return;
                        }

                        var window = new RemoveMsgBox($"Удалить узел {SelectedComputeNode.Ip}?");
                        if (!(bool)window.ShowDialog()) return;

                        string removeIp = SelectedComputeNode?.Ip;
                        ComputeNodes.Remove(SelectedComputeNode);
                        if (ComputeNodes != null && ComputeNodes.Count > 0)
                            SelectedComputeNode = ComputeNodes.First();

                        SetButtonsEndbled();
                        SetNodesInformationLog();
                        SetResultInformationlog($"(!) Узел {removeIp} удален из списка");
                    }));
            }
        }

        // проверка соединения к узлу
        private bool CheckConnection()
        {
            DateTime connectStart = DateTime.Now;
            string url = $"{SelectedProtocol.ProtValue}{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}:{SelectedProtocol.Port}/ComputeNodeServiceLib.ComputeNodeService";
            
            Binding binding = null;
            if (SelectedProtocol.Id == 0)
                binding = new BasicHttpBinding();
            else
                binding = new NetTcpBinding();

            var address = new EndpointAddress(url);

            using (var proxy = new ComputeNodeService.ComputeNodeClient(binding, address))
            {
                try
                {
                    bool result = proxy.IsWorking();
                    SetResultInformationlog($"[connect] Подключение к {FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart} установлено! Время подключения к узлу: { (DateTime.Now - connectStart).Milliseconds } м/сек");
                    return result;
                }
                catch (Exception e)
                {
                    SetResultInformationlog($"[disconnect] Не удалось подключиться к {FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart} в течении { (DateTime.Now - connectStart).Milliseconds } м/сек.");
                    SetResultInformationlog($"[error]: {e.Message}");
                    throw;
                }
            }
        }

        // номер узла
        private static int nodesNum = 1;

        // метод добавления нового узла
        private void AddNewComputeNode()
        {
            if (ComputeNodes == null)
                ComputeNodes = new ObservableCollection<ComputeNode>();

            ComputeNodes.Add(new ComputeNode
            {
                Id = nodesNum++,
                ConnectionDate = DateTime.Now,
                Protocol = SelectedProtocol,
                Ip = $"{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}",
                Url = $"{SelectedProtocol.ProtValue}{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}:{SelectedProtocol.Port}/ComputeNodeServiceLib.ComputeNodeService"
            });

            SelectedComputeNode = ComputeNodes.First();
        }


        // настройка enabled кнопок контролера
        private void SetButtonsEndbled(bool startCompute = false)
        {
            bool addNodeBtn = false, removeNodeBtn = false, startComputeBtn = false;
            if (startCompute)
            {
                addNodeBtn = removeNodeBtn = startComputeBtn = false;
            }
            else
            {
                addNodeBtn = true;
                removeNodeBtn = ComputeNodes != null && ComputeNodes.Count > 0 ? true : false;
                startComputeBtn = !startCompute;
            }

            AddNodeBtnEnabled = addNodeBtn;
            RemoveNodeBtnEnabled = removeNodeBtn;
            StartComputeBtnEnabled = startComputeBtn;
            CancelComputeBtnEnabled = startCompute;
        }


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
            else
            {
                nodesInfo = $"Количество подключенных узлов: {ComputeNodes.Count}";
                foreach (var cn in ComputeNodes)
                    nodesInfo += $"\n  - Узел №{cn.Id}:  {cn.Url}  [{cn.Protocol?.Name}]  [{cn.ConnectionDate}]";
            }

            NodesInformationLog = nodesInfo;
        }

        private void SetResultInformationlog(string message)
        {
            ResultInformationlog += $"\n{DateTime.Now}: {message}";
        }

        private void SetNodesComputeOutputLog(RestorePasswordResult result, string node)
        {
            string isFounded = result.PasswordIsRestored ? "found" : "not found";
            string nodesComputeInfo = $"[{isFounded}] Результат работы узла {node}:";
            nodesComputeInfo += $"\n - Время затраченное на подбор пароля: {result.OperationsLeadTime} м/сек";
            nodesComputeInfo += $"\n - Количество операций на подбор пароля для узла: {result.OperationCount}";
            nodesComputeInfo += $"\n - Пароль восстановлен: {result.PasswordIsRestored}";
            nodesComputeInfo += $"\n - Восстановленный пароль: {result.RestorePassword} \n\n";

            NodeComputeOutput += nodesComputeInfo;
        }
    }
}
