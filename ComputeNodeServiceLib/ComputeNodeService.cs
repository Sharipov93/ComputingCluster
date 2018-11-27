using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComputeNodeServiceLib
{
    public class ComputeNodeService : IComputeNode
    {
        public bool IsWorking() => true;

        // хэш пароля, высылаемый службе и результат найденного пароля
        private string hash, resultPassword;

        // флаг показывающай найден ли пароль
        private bool isMatched = false;

        // Длина массива startSymbolsRange для повышения производительности
        private int startSymbolsRangeLength = 0;
        private long computedKeys = 0;

        // Пересылаемый массив содержаищий символы, по которым будет подбираться пароль
        private char[] charactersToRestorePass;


        /// <summary>
        /// Основной метод сервиса, к этому методу будут обращаться удаленные клиенты
        /// </summary>
        /// <param name="passwordHash">Хэш пароля</param>
        /// <param name="startSymbolsRange">стартовый массив символов</param>
        public RestorePasswordResult RestorePassword(string passwordHash, char[] startSymbolsRange)
        {
            // Время начала подбора пароля в м/сек
            DateTime startTime = DateTime.Now;

            hash = passwordHash;
            charactersToRestorePass = startSymbolsRange;

            // длина символьного массива
            startSymbolsRangeLength = startSymbolsRange.Length;

            // т.к. длина пароля не известна, будем подбирать все возможные комбинации
            int estimatedPasswordLength = 0;

            while (!isMatched)
            {
                estimatedPasswordLength++;
                startBruteForce(estimatedPasswordLength);
            }

            return new RestorePasswordResult
            {
                OperationCount = computedKeys,
                OperationsLeadTime = (DateTime.Now - startTime).Milliseconds,
                PasswordHash = passwordHash,
                PasswordIsRestored = isMatched,
                RestorePassword = resultPassword
            };
        }


        /// <summary>
        /// Рекурсивный метод для подбора возможный значений
        /// </summary>
        /// <param name="keyLength">Длина ключа</param>
        private void startBruteForce(int keyLength)
        {
            var keyChars = createCharArray(keyLength, charactersToRestorePass[0]);
            // индекс последнего символа будет сохранен для небольшого улучшения производительности
            var indexOfLastChar = keyLength - 1;
            createNewKey(0, keyChars, keyLength, indexOfLastChar);
        }

        /// <summary>
        /// Создает новый массив символов длины length, заполненный параметром defaultChar
        /// </summary>
        /// <param name="length">Длинна массива</param>
        /// <param name="defaultChar">Символ, которым будет заполнен массив</param>
        /// <returns></returns>
        private char[] createCharArray(int length, char defaultChar)
        {
            return (from c in new char[length] select defaultChar).ToArray();
        }


        /// <summary>
        /// Основной метод подбора пароля, он создает новые ключи, генрирует для них хэш и проверяет с отправленным хэшом пароля
        /// до тех пор пока не найдет парель либо не осущиствит перебер через весь массив символов
        /// </summary>
        /// <param name="currentCharPosition">Позиция символа, который заменятся новым</param>
        /// <param name="keyChars">Текущий ключ представленный как массив символов</param>
        /// <param name="keyLength">Длина ключа</param>
        /// <param name="indexOfLastChar">Индекс последнего символа ключа</param>
        /// 
        private void createNewKey(int currentCharPosition, char[] keyChars, int keyLength, int indexOfLastChar)
        {
            var nextCharPosition = currentCharPosition + 1;
            // Перебираем через весь наш массив стартовых символов charactersToRestorePass
            for (int i = 0; i < startSymbolsRangeLength; i++)
            {
                /* Сивмол в currentCharPosition будет заменен новым символом
                 * из charactersToRestorePass массива => будет создана новая комбинация ключа */
                keyChars[currentCharPosition] = charactersToRestorePass[i];

                // Метод будет вызывать себя рекурсивно пока не переберет все комбинации массива символов
                if (currentCharPosition < indexOfLastChar)
                {
                    createNewKey(nextCharPosition, keyChars, keyLength, indexOfLastChar);
                }
                else
                {
                    // Количество операций
                    computedKeys++;

                    // генерация нового пароля в виде строки и создание хэша для него
                    string genPass = new String(keyChars);
                    var hashString = BitConverter.ToString(GenerateHashPassword(genPass))?.Replace("-", "")?.ToLower();

                    /* проеряем сгенерированный хэш с хэшом - пароля отправленного сервису,
                     * если хэши совпадают завершаем выполнение
                     */
                    if (hashString == hash)
                    {
                        if (!isMatched)
                        {
                            isMatched = true;
                            resultPassword = genPass;
                        }
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Метод генерации хэш - кода для ключа
        /// </summary>
        /// <param name="s">Ключ (подбираемый пароль)</param>
        private byte[] GenerateHashPassword(string s)
        {
            var utf8 = Encoding.UTF8;
            var data = utf8.GetBytes(s);
            byte[] passwordHash = null;
            try
            {
                using (var alg = new SHA256Managed())
                {
                    passwordHash = alg.ComputeHash(data);
                }
            }
            catch
            {
                passwordHash = null;
            }

            return passwordHash;
        }
    }
}
