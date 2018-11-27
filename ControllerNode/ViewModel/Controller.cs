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
                char invalidChar;
                if (!CheckLastInputSymbol(value, out invalidChar))
                {
                    new InformationMessageBox($"Допустимые символы: [0 - 9], [A - Z], [a - z]").ShowDialog();
                    if (invalidChar != '!')
                        value = value.Remove(value.IndexOf(invalidChar), 1);
                }

                if (!string.IsNullOrEmpty(value))
                    Hash = BitConverter.ToString(GenerateHashPassword(value))?.Replace("-", "")?.ToLower();
                else
                    Hash = "";
                SetButtonsEndbled();

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

        private IList<ComputeNodeService.ComputeNodeClient> channels;

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
                        SetButtonsEndbled();

                        if (timer != null)
                            timer.Change(Timeout.Infinite, 0);

                        if (channels == null || channels.Count <= 0) return;
                        foreach (var channel in channels)
                        {
                            if (channel.State == CommunicationState.Created ||
                                channel.State == CommunicationState.Opening || channel.State == CommunicationState.Opened)
                                continue;

                            try
                            {
                                channel.Abort();
                            }
                            catch { }
                        }

                        channels = null;
                    }));
            }
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

        // массив допустимых символов в пароле
        private char[] allowSymbols = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z'
             };

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

        private void RestorePassword()
        {
            AllProcessTime = 0;

            timer = new Timer(obj =>
            {
                AllProcessTime++;
            }, null, 0, 1000);

            channels = new List<ComputeNodeService.ComputeNodeClient>();
            foreach (var node in ComputeNodes)
            {
                SetResultInformationlog($"Отправлен запрос на вычисление на узел {node.Url}, время отправки: {DateTime.Now}");

                Binding binding = null;
                if (node.Protocol.Id == 0)
                    binding = new BasicHttpBinding();
                else
                    binding = new NetTcpBinding();

                var address = new EndpointAddress(node.Url);

                var proxy = new ComputeNodeService.ComputeNodeClient(binding, address);
                channels.Add(proxy);

                try
                {
                    using (proxy)
                    {
                        IAsyncResult result = proxy.BeginRestorePassword(Hash, allowSymbols, 
                            ar =>
                            {
                                try
                                {
                                    var computeResult = proxy.EndRestorePassword(ar);

                                    SetResultInformationlog($"(!) Узел {node.Url} завершил работу: {DateTime.Now}");
                                    SetNodesComputeOutputLog(computeResult, node.Url);
                                    SetButtonsEndbled();
                                    timer.Change(Timeout.Infinite, 0);
                                    MessageBox.Show(computeResult.RestorePassword);
                                }
                                catch { }
                            }, null);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
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

                        string ip = $"{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}";
                        bool existNode = ComputeNodes != null && ComputeNodes.Where(c => 
                            c.Ip.Contains(ip)).Count() > 0 ? true : false;

                        if (existNode)
                        {
                            new InformationMessageBox($"Приложение уже содержит узел: {ip}").ShowDialog();
                            return;
                        }

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
        private bool CheckLastInputSymbol(string password, out char invalidSymbol)
        {
            var isValidChars = true;
            invalidSymbol = '!';

            if (string.IsNullOrEmpty(password)) return isValidChars;

            foreach (char c in password)
            {
                if (char.IsDigit(c) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122))
                    continue;
                
                isValidChars = false;
                invalidSymbol = c;
            }

            return isValidChars;
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
