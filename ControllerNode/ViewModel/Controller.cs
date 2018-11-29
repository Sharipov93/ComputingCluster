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
        // кнопка добавления узла
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

        // кнопка удлаения узла
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

        // кнопка запуска вычисления
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

        // кнопка пользовательской отмены
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


        // поле информации использования
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

        // поле, содержащее информацию об подключенныз узлах
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

        // лог выполнения вычислений и ошибок
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

        // результат работы узлов
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

        // пароль, вводимый пользователем
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

        // ip - адрес, состоит из 4 частей
        private string firstIpPart;
        public string FirstIpPart
        {
            get { return firstIpPart; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;

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
                if (string.IsNullOrEmpty(value)) return;

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
                if (string.IsNullOrEmpty(value)) return;

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
                if (string.IsNullOrEmpty(value)) return;

                int intVal = Int32.Parse(value);
                if (intVal > 255) value = "255";


                fourthIpPart = value;
                NotifyPropertyChanged("FourthIpPart");
            }
        }


        // коллекция протоколов передачи данных
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
        

        /// <summary>
        /// Загрузка допустимых протоколов
        /// </summary>
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
        // выбранный узел
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


        /// <summary>
        /// Команда валидации ввода ip - адреса
        /// </summary>
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

                        // если введенный символ не цифра, удаляем его
                        char[] symbols = currentText.ToCharArray();
                        foreach (var s in symbols)
                            if (!char.IsDigit(s))
                                validateTxtBox.Text = currentText.Remove(currentText.IndexOf(s), 1);
                    }));
            }
        }

 
        /// <summary>
        /// Команда запуска узлов для восстановления пароля
        /// </summary>
        private ControllerCommand restorePasswordCommand;
        public ControllerCommand RestorePasswordCommand
        {
            get
            {
                return restorePasswordCommand ??
                    (restorePasswordCommand = new ControllerCommand(obj =>
                    {
                        // валидация введенных данных
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
                        
                        // генерация хэш-кода для пароля
                        Hash = BitConverter.ToString(GenerateHashPassword(Password))?.Replace("-", "")?.ToLower();

                        // сохраняем запись в лог и устанавливаем видимость кнопок
                        SetResultInformationlog($"Сгенерирован хэш-код для введеного пароля [{Password}]: {Hash}");
                        SetButtonsEndbled(true);

                        // запускаем метод, который будет обращаться к сервисам за подбором пароля.
                        // метод запускается в параллельном потоке
                        var task = Task.Factory.StartNew(RestorePassword);
                    }));
            }
        }
        

        /// <summary>
        /// Команда пользовательской отмены работы узлов
        /// </summary>
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

                        SetResultInformationlog("(!) Пользователь отменил выполнение. Завершилось выполнение всех узлов".ToUpper());

                        cancelCompute();
                    }));
            }
        }


        /// <summary>
        /// метод останавливает работу сервисов
        /// </summary>
        private void cancelCompute()
        {
            SetButtonsEndbled();

            // если таймер запущен, останавливаем его
            if (timer != null)
                timer.Change(Timeout.Infinite, 0);
            
            if (channels == null || channels.Count <= 0) return;
            foreach (var channel in channels)
            {
                if (channel.State != CommunicationState.Closed)
                {
                    try
                    {
                       // если канал не закрыт, отправляем сообщение сервису о прекращении работы
                       // сообщение передается в ассинхронном виде
                        IAsyncResult result = channel.BeginStopPasswordComputing(ar => {}, null);
                    }
                    catch { }
                }
            }

            channels = null;
        }


        /// <summary>
        /// метод генерации хэш-код пароля
        /// </summary>
        /// <param name="password">введенный пароль</param>
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
                MessageBox.Show($"Ошибка при хэшировании пароля.\n{e.Message}");
            }

            return passwordHash;
        }

        // строка допустимых символов в пароле. При передаче серверу будет преобразованна в char[]
        private string allowSymbols = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static int startIndex;
        private static int startSymbolsCount;

        // таймер для подсчета общего времени всего процесса восстановления пароля
        private Timer timer;

        // свойство отображения времени
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

        // коллекция, которая хранит каналы подключений к сервисам
        private IList<ComputeNodeService.ComputeNodeClient> channels;

        // общее время вычислений на узлах
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

        // общее количество вычислений на узлах
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


        /// <summary>
        /// метод восстановления пароля
        /// </summary>
        private void RestorePassword()
        {
            AllNodeTime = 0;
            AllNodeKeys = 0;
            AllProcessTime = 0;

            timer = new Timer(obj =>
            {
                AllProcessTime++;

                // по истечинию 30 секунд, завершаем работу сервисов и уведомляем об этом пользователя
                if (AllProcessTime >= 30)
                {
                    SetResultInformationlog($"(!) Истек таймаут ({AllProcessTime}секунд). Пароль не подобран.".ToUpper());
                    cancelCompute();
                }
            }, null, 0, 1000);

            // вычисляем кол-во элементов, которые будут распределены между узлами 
            startIndex = 0;
            startSymbolsCount = allowSymbols.Length / ComputeNodes.Count;

            // коллекция каналов сервисов
            channels = new List<ComputeNodeService.ComputeNodeClient>();

            foreach (var node in ComputeNodes)
            {
                // создаем wcf привязку 
                Binding binding = null;
                if (node.Protocol.Id == 0)
                    binding = new BasicHttpBinding();
                else
                    binding = new NetTcpBinding();

                // адрес хоста
                var address = new EndpointAddress(node.Url);

                // создаем канал-клиента, по которому будем обращаться к сервису и добавляем канал в коллекцию
                var proxy = new ComputeNodeService.ComputeNodeClient(binding, address);
                channels.Add(proxy);

                // создаем массив стартовых символов, которые будут распределены для каждого узла
                string startSymbols = allowSymbols.Substring(startIndex, startSymbolsCount);
                startIndex += startSymbolsCount;

                SetResultInformationlog($"Отправлен запрос на вычисление на узел {node.Url}: \n - Стартовый массив символов: [{startSymbols}]");

                try
                {
                    // отправляем сервису хэш-код пароля и массив стартовых символов.
                    // запрос отправляется ассинхронным образом
                    // тут же создаем метод обратного вызова от службы
                    IAsyncResult result = proxy.BeginRestorePassword(Hash, startSymbols.ToCharArray(),
                        ar =>
                        {
                            try
                            {
                                // получаем результат работы сервиса
                                var computeResult = proxy.EndRestorePassword(ar);

                                // уведомляем пользователя о результате работы узла
                                SetResultInformationlog($"(!) Узел {node.Url} завершил работу: {DateTime.Now}");
                                SetNodesComputeOutputLog(computeResult, node.Url);
                                SetButtonsEndbled();

                                // останавливаем работу таймера
                                timer.Change(Timeout.Infinite, 0);

                                // считаем общее кол-во времени работы и общее кол-во операций по подбору пароля всех узлов
                                AllNodeTime += computeResult.OperationsLeadTime;
                                AllNodeKeys += computeResult.OperationCount;

                                // если ответ от узла положителен, т.е. сервис смог подобрать пороль, то
                                // уведомляем остальные узлы о прекращеннии работы.
                                if (computeResult.PasswordIsRestored)
                                {
                                    foreach (var c in channels)
                                    {
                                        if (c.State != CommunicationState.Closed)
                                        {
                                            var r = c.BeginStopPasswordComputing(cr => { }, null);
                                        }
                                    }

                                    MessageBox.Show($"Подобранный пароль: {computeResult.RestorePassword}");
                                }
                            }
                            catch { }

                        }, null);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


        /// <summary>
        /// Команда добавления нового узла
        /// </summary>
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

                        // Расскомментировать этот код, если нужна проверка на исключение дублирования узлов
                        // Т.е. нельзя будет добавить 2 узла с одним и тем же ip - адресом
                        //string ip = $"{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}";
                        //bool existNode = ComputeNodes != null && ComputeNodes.Where(c =>
                        //    c.Ip.Contains(ip)).Count() > 0 ? true : false;

                        //if (existNode)
                        //{
                        //    new InformationMessageBox($"Приложение уже содержит узел: {ip}").ShowDialog();
                        //    return;
                        //}

                        // проверяем подключение к узлу.
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

                        // если удается подключиться добавляем узел в систему
                        AddNewComputeNode();

                        SetButtonsEndbled();
                        SetNodesInformationLog();
                    }));
            }
        }


        /// <summary>
        /// метод валидации вводимого пароля
        /// </summary>
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


        /// <summary>
        /// Команда удаления узла из приложения
        /// </summary>
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

                        // если пользователь подтвердил, то удаляем узел из коллекции
                        string removeIp = SelectedComputeNode?.Ip;
                        ComputeNodes.Remove(SelectedComputeNode);
                        if (ComputeNodes != null && ComputeNodes.Count > 0)
                            SelectedComputeNode = ComputeNodes.First();

                        // уведомляем пользователя об этом
                        SetButtonsEndbled();
                        SetNodesInformationLog();
                        SetResultInformationlog($"(!) Узел {removeIp} удален из списка");
                    }));
            }
        }



        /// <summary>
        /// метод проверки подключения к службе
        /// </summary>
        private bool CheckConnection()
        {
            // дата подключения и url хоста
            DateTime connectStart = DateTime.Now;
            string url = $"{SelectedProtocol.ProtValue}{FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart}:{SelectedProtocol.Port}/ComputeNodeServiceLib.ComputeNodeService";
            
            // создаем wcf привязку
            Binding binding = null;
            if (SelectedProtocol.Id == 0)
                binding = new BasicHttpBinding();
            else
                binding = new NetTcpBinding();

            // адрес сервиса
            var address = new EndpointAddress(url);

            // создаем прокси и отправляем запрос на службу. 
            // результат подключения будет отображен в логе
            using (var proxy = new ComputeNodeService.ComputeNodeClient(binding, address))
            {
                try
                {
                    bool result = proxy.IsWorking();
                    SetResultInformationlog($"[connect] Подключение к {FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart} установлено! Время подключения к узлу: { (DateTime.Now - connectStart).Milliseconds } миллисекунд");
                    return result;
                }
                catch (Exception e)
                {
                    SetResultInformationlog($"[disconnect] Не удалось подключиться к {FirstIpPart}.{SecondIpPart}.{ThirdIpPart}.{FourthIpPart} в течении { (DateTime.Now - connectStart).Milliseconds } миллисекунд");
                    SetResultInformationlog($"[error]: {e.Message}");
                    throw;
                }
            }
        }


        // номер узла
        private static int nodesNum = 1;


        /// <summary>
        /// метод добавления нового узла
        /// </summary>
        private void AddNewComputeNode()
        {
            // если коллекции не существует, то создаем ее
            if (ComputeNodes == null)
                ComputeNodes = new ObservableCollection<ComputeNode>();

            // добавляем узел в коллекцию
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


        /// <summary>
        /// метод который настраивет enabled свойство кнопок приложения
        /// </summary>
        /// <param name="startCompute">состояние приложения. true -> выполнение</param>
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


        /// <summary>
        /// метод информации об использованнии приложением
        /// </summary>
        private void SetInformationLog()
        {
            string instruction = "1. Запустите консольное приложение(КП -> выполняет роль хоста службы WCF) на необходимых компьютерах.";
            instruction += "\n2. Для подключения к хосту, добавьте 1(+) вычислительных узлов.";
            instruction += "\n   (При добавлении узла, укажите ip-адрес компьютера, где выполняется хост и протокол передачи данных (http, tcp))";
            instruction += "\n3. Введите пароль. (Допустимые символы: { [A - Z], [a - z], [0 - 9] })";
            instruction += "\n4. Нажмите кнопку 'Запустить'";

            UseInformationLog = instruction;
        }


        /// <summary>
        /// метод отоброжает в консоли кол-во и др. информацию об подключенных узлах
        /// ComputeNodes -> коллекция узлов
        /// </summary>
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


        /// <summary>
        /// выводит в консоль информацию о ходе работы приложения
        /// </summary>
        private void SetResultInformationlog(string message)
        {
            ResultInformationlog += $"\n{DateTime.Now}: {message}";
        }


        /// <summary>
        /// отображает в консоль ответ от узла.
        /// </summary>
        /// <param name="result">ответ полученный от сервиса</param>
        /// <param name="node">сервис, от которого получен результат</param>
        private void SetNodesComputeOutputLog(RestorePasswordResult result, string node)
        {
            string isFounded = result.PasswordIsRestored ? "found" : "not found";
            string nodesComputeInfo = $"[{isFounded}] Результат работы узла {node}:";
            nodesComputeInfo += $"\n - Время затраченное на подбор пароля: {result.OperationsLeadTime} миллисекунд";
            nodesComputeInfo += $"\n - Количество операций на подбор пароля для узла: {result.OperationCount}";
            nodesComputeInfo += $"\n - Пароль восстановлен: {result.PasswordIsRestored}";
            nodesComputeInfo += $"\n - Восстановленный пароль: {result.RestorePassword} \n\n";

            NodeComputeOutput += nodesComputeInfo;
        }
    }
}
