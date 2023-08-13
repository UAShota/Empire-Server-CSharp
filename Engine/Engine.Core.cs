/////////////////////////////////////////////////
//
// Модуль управления ядром
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Empire.Database;
using NLog;

namespace Empire.EngineSpace
{
    /// <summary>
    /// Движок управления подсистемами
    /// </summary>
    internal static class Core
    {
        /// <summary>
        /// Параметры командной строки
        /// </summary>
        private enum CmdParams
        {
            /// <summary>
            /// Имя базы данных
            /// </summary>
            DbName,
            /// <summary>
            /// Имя пользователя базы данных
            /// </summary>
            DbUser,
            /// <summary>
            /// Пароль базы данных
            /// </summary>
            DbPwd,
            /// <summary>
            /// Порт сервера
            /// </summary>
            ServerPort
        }

        /// <summary>
        /// Строка лога запуска модуля
        /// </summary>
        private const string csLogWatchStart = "{0}";

        /// <summary>
        /// Строка лога времени 
        /// </summary>
        private const string csLogWatchStop = "{0}: {1} msec";

        /// <summary>
        /// Экземпляр класс логирования
        /// </summary>
        private static EngineLog fLog { get; set; }

        /// <summary>
        /// Экземпляр управления тасками
        /// </summary>
        private static TaskFactory fTasks { get; set; }

        /// <summary>
        /// Доступ к базе данных
        /// </summary>
        public static CustomDatabase Database { get; private set; }

        /// <summary>
        /// Доступ к серверному функционалу
        /// </summary>
        public static EngineServer Server { get; private set; }

        /// <summary>
        /// Доступ к логированию
        /// </summary>
        public static Logger Log => fLog.Log;

        /// <summary>
        /// Логирование сведений
        /// </summary>
        /// <param name="aFormat">Формат строки</param>
        /// <param name="aValues">Массив параметров</param>
        private static void Watch(string aFormat, params object[] aValues)
        {
            if ((fLog != null) && (fLog.Log != null))
                fLog.Log.Info(aFormat, aValues);
            else
                Console.WriteLine(aFormat, aValues);
        }

        /// <summary>
        /// Запуск сервера
        /// </summary>
        public static void Start()
        {
            Core.CheckError("Starting core", () =>
            {
                string tmpDbName = CommonCmd.AsString(CmdParams.DbName);
                string tmpDbUser = CommonCmd.AsString(CmdParams.DbUser);
                string tmpDbPwd = CommonCmd.AsString(CmdParams.DbPwd);
                int tmpPort = CommonCmd.AsInt(CmdParams.ServerPort);
                // Создадим объекты
                fTasks = new TaskFactory();
                StartAction(nameof(EngineLog), () => fLog = new EngineLog());
                StartAction(nameof(MySqlDatabase), () => Database = new MySqlDatabase(tmpDbName, tmpDbUser, tmpDbPwd));
                StartAction(nameof(EngineServer), () => Server = new EngineServer(tmpPort));
            });
        }

        /// <summary>
        /// Остановка сервера
        /// </summary>
        public static void Stop()
        {
            StartAction(nameof(EngineServer), () => Server.Dispose());
            StartAction(nameof(MySqlDatabase), () => Database.Dispose());
            StartAction(nameof(EngineLog), () => fLog.Dispose());
        }

        /// <summary>
        /// Запуск потока выполнения
        /// </summary>
        /// <param name="aAction">Код выполнение</param>
        /// <returns>Задача потока выполнения</returns>
        public static Task StartTask(Action aAction)
        {
            return fTasks.StartNew(aAction);
        }

        /// <summary>
        /// Запуск действия с логированием времени выполнения
        /// </summary>
        /// <param name="aModule">Имя модуля</param>
        /// <param name="aAction">Код выполнения</param>
        public static void StartAction(string aName, Action aAction)
        {
            Watch(csLogWatchStart, aName);
            Stopwatch tmpWatch = new Stopwatch();
            tmpWatch.Start();
            aAction();
            Watch(csLogWatchStop, aName, tmpWatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Запуск кода с проверкой на фатальную ошибку
        /// </summary>
        /// <param name="aName">Имя кода</param>
        /// <param name="aAction">Каллбак выполнения</param>
        public static void CheckError(string aName, Action aAction)
        {
            try
            {
                aAction();
            }
            catch (Exception E)
            {
                Watch("{0} {1}", E.Message, E.StackTrace);
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }
    }
}