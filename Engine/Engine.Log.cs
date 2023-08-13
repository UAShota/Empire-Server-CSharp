/////////////////////////////////////////////////
//
// Модуль управления подсистемой логирования 
//
// Copyright(c) 2016 UAShota
//
// Rev I  2020.05.01

//
/////////////////////////////////////////////////

using System;
using System.IO;
using NLog;

namespace Empire.EngineSpace
{
    /// <summary>
    /// Класс логирования
    /// </summary>
    internal class EngineLog : IDisposable
    {
        /// <summary>
        /// Каталог логов
        /// </summary>
        private const string csLogDir = "Logs";

        /// <summary>
        /// Расширение файла лога
        /// </summary>
        private const string csLogExt = ".log";

        /// <summary>
        /// Формат имени лога
        /// </summary>
        private const string csLogName = "yyyy.MM.dd";

        /// <summary>
        /// Формат лога для вывода в консоли
        /// </summary>
        private const string csLogLayout = "${time}|${level}|${callsite}:${callsite-linenumber}|${message}";

        /// <summary>
        /// Формат лога для вывода в файл
        /// </summary>
        private const string csLogLayoutTrace = csLogLayout + "|${stacktrace}";

        /// <summary>
        /// Экземпляр логирования
        /// </summary>
        public Logger Log { get; private set; }

        /// <summary>
        /// Запуск логирования
        /// </summary>
        public EngineLog()
        {
            // Сформируем имя файла лога
            var tmpFileName = Path.Combine(Directory.GetCurrentDirectory(), csLogDir, DateTime.Now.ToString(csLogName) + csLogExt);
            // Запись событий в файл
            var tmpLogfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = tmpFileName,
                Layout = csLogLayout
            };
            // Запись исключений в файл
            var tmpLogfileTrace = new NLog.Targets.FileTarget("logfile")
            {
                FileName = tmpFileName,
                Layout = csLogLayoutTrace
            };
            // Запись событий в консоль
            var tmpLogConsole = new NLog.Targets.ConsoleTarget("logconsole")
            {
                Layout = csLogLayout
            };
            // Добавим правила
            var tmpConfig = new NLog.Config.LoggingConfiguration();
            tmpConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, tmpLogConsole);
            tmpConfig.AddRule(LogLevel.Info, LogLevel.Warn, tmpLogfile);
            tmpConfig.AddRule(LogLevel.Error, LogLevel.Fatal, tmpLogfileTrace);
            LogManager.Configuration = tmpConfig;
            // Заберем инстанс
            Log = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Остановка логирования
        /// </summary>
        public void Dispose()
        {
            LogManager.Shutdown();
            Log = null;
        }
    }
}