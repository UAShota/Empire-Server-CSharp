/////////////////////////////////////////////////
//
// Класс обработки параметров командной строки
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Empire
{
    /// <summary>
    /// Базовый класс работы с параметрами командной строки
    /// </summary>
    public class CommonCmd
    {
        /// <summary>
        /// Парсер командной строки сохраняет в себе строку и набор параметров в виде ключ/значение
        /// </summary>
        private class CmdParser
        {
            #region Переменные
            /// <summary>
            /// Разделитель ключ:значение
            /// </summary>
            private const char ccKeyValue = ':';

            /// <summary>
            /// Разделитель блоков
            /// </summary>
            private const char ccSpace = ' ';

            /// <summary>
            /// Разделитель квотированных блоков
            /// </summary>
            private const char ccQuote = '"';

            /// <summary>
            /// Символ начала ключа
            /// </summary>
            private const string csSeparators = "-/";

            /// <summary>
            /// Текущая позиция в блоке
            /// </summary>
            private int fPosition;

            /// <summary>
            /// Позиция начала блока
            /// </summary>
            private int fStart;

            /// <summary>
            /// Текущий флаг блока
            /// </summary>
            private CmdParserItem fFlag = null;

            /// <summary>
            /// Признак активного флага
            /// </summary>
            private bool fFlagActive = false;

            /// <summary>
            /// Признак активного разделителя
            /// </summary>
            private bool fSeparatorActive;

            /// <summary>
            /// Признак пути без кавычек, первый параметр строко через разделитель
            /// </summary>
            private bool fUnquotedPath;
            #endregion

            #region Приватные методы
            /// <summary>
            /// Признак завершения парсинга
            /// </summary>
            private bool EndOfText()
            {
                return fPosition >= CommandLine.Length;
            }

            /// <summary>
            /// Возвращение символа в указанной позиции
            /// </summary>
            private char Current()
            {
                return fPosition < CommandLine.Length ? CommandLine[fPosition] : (char)0;
            }

            /// <summary>
            /// Возвращение предыдущего символа от текущей позиции
            /// </summary>
            private char Prev()
            {
                return fPosition > 0 ? CommandLine[fPosition - 1] : (char)0;
            }

            /// <summary>
            /// Возвращение символа в следующей позиции
            /// </summary>
            private char MoveAhead()
            {
                char tmpChar = Current();
                fPosition = Math.Min(fPosition + 1, CommandLine.Length);
                return tmpChar;
            }

            /// <summary>
            /// Копирование текущего блока
            /// </summary>
            /// <returns>Строка текущего блока</returns>
            private string Copy()
            {
                return CommandLine.Substring(fStart, fPosition - fStart);
            }

            /// <summary>
            /// Разбор текущего блока
            /// </summary>
            private void ParseParam()
            {
                // Первый блок всегда имя файла
                if (FileName == null)
                {
                    FileName = Copy();
                    if (FileName.StartsWith(ccQuote.ToString()))
                        FileName = FileName.Substring(1, FileName.Length - 2);
                    return;
                }
                // Если нет флага открывающего параметр
                if (!fFlagActive)
                {
                    Flags.Add(new CmdParserItem() { Key = Copy() });
                    return;
                }
                // Если есть флаг, то определим ключ это или значение
                if (fFlag != null)
                {
                    // Запишем значения флага
                    fFlag.Value = Copy();
                    Flags.Add(fFlag);
                    // Подготовимся к ожиданию нового флага
                    fFlag = null;
                    fFlagActive = false;
                    fSeparatorActive = false;
                }
                else
                    fFlag = new CmdParserItem() { Key = Copy() };
            }

            /// <summary>
            /// Разбор текущего блока
            /// </summary>
            /// <param name="aEndChar">Ожидаемый символ разделения блока</param>
            private void ParseBlock(char aEndChar)
            {
                while (!EndOfText())
                {
                    // Поищем окончание блока или при активном флаге разделитель параметров
                    if (Current() == aEndChar || fFlagActive && Current() == ccKeyValue)
                    {
                        // Команда запуска без кавычек должна иметь хотя бы первый параметр с разделителем
                        if (fUnquotedPath && Current() == ccSpace)
                        {
                            MoveAhead();
                            continue;
                        }
                        // Завершили блок
                        if (Current() == aEndChar)
                            fSeparatorActive = false;
                        // Если нет активного разделитея, пропустим ключ:параметр
                        if (!fSeparatorActive && Current() == ccKeyValue)
                        {
                            MoveAhead();
                            continue;
                        }
                        // Если есть активный разделитель и есть ключ:параметр
                        if (fSeparatorActive && Current() == ccKeyValue)
                        {
                            // Разберем блок и перейдем дальше
                            ParseParam();
                            MoveAhead();
                            fSeparatorActive = false;
                            fStart = fPosition;
                            // Если последний символ кавычки, то ищем закрывающие
                            if (Current() == ccQuote)
                            {
                                aEndChar = ccQuote;
                                MoveAhead();
                            }
                            continue;
                        }
                        // Если текущий блок заканчивается на кавычку - передвинем дальше
                        if (Current() == ccQuote)
                            MoveAhead();
                        // Разберем итоговый блок
                        ParseParam();
                        break;
                    }
                    // Если есть разделитель команд и перед ним пробел - начнем блок
                    else if (csSeparators.Contains(Current().ToString()) && Prev() == ccSpace)
                    {
                        // Если была команда сбора строки без кавычек - сохраним имя файла
                        if (fUnquotedPath)
                        {
                            ParseParam();
                            fUnquotedPath = false;
                        }
                        // Если команда уже есть - сохраним и начнем новую
                        if (fFlagActive)
                        {
                            Flags.Add(fFlag);
                            fFlag = null;
                        }
                        else
                            fFlagActive = true;
                        fSeparatorActive = true;
                        break;
                    }
                    MoveAhead();
                }
                // Если вся подстрока высчитана - остаток закидываем
                if (EndOfText())
                {
                    if (Prev() != ccQuote)
                        ParseParam();
                    if (fFlag != null)
                        Flags.Add(fFlag);
                }
                else
                    MoveAhead();
            }

            /// <summary>
            /// Убрать символ обрамления, если он есть по краям строки
            /// </summary>
            /// <param name="aStr">
            /// Строка
            /// </param>
            /// <param name="aQuote">
            /// Символ обрамления
            /// </param>
            /// <param name="aOnlyDoubleQuote">
            /// Если true - удаляется только при наличии символа обрамления с двух сторон, 
            /// иначе - при наличии на любой стороне
            /// </param>
            /// <returns>
            /// Исходная или обработанная строка
            /// </returns>
            private void DequotedStr(StringBuilder aStr, char aQuote = '"', bool aOnlyDoubleQuote = true)
            {
                // Проверка необходимости обработки строки
                if (aStr.Length == 0)
                    return;
                // Проверка режима удаления обрамления. В паре, либо с любой из сторон.
                if ((!aOnlyDoubleQuote && aStr[0] != aQuote && (aStr[aStr.Length - 1] != aQuote))
                    || (aOnlyDoubleQuote && (aStr.Length < 2 || aStr[0] != aQuote || aStr[aStr.Length - 1] != aQuote)))
                    return;
                // В начале строки
                if (aStr[0] == aQuote)
                    aStr.Remove(0, 1);
                // В конце 
                if (aStr.Length > 0 && aStr[aStr.Length - 1] == aQuote)
                    aStr.Remove(aStr.Length - 1, 1);
            }

            /// <summary>
            /// Убрать символ обрамления, если он есть по краям строки
            /// </summary>
            /// <param name="aStr">
            /// Строка
            /// </param>
            /// <param name="aQuote">
            /// Символ обрамления
            /// </param>
            /// <param name="aOnlyDoubleQuote">
            /// Если true - удаляется только при наличии символа обрамления с двух сторон, 
            /// иначе - при наличии на любой стороне
            /// </param>
            /// <returns>
            /// Исходная или обработанная строка
            /// </returns>
            private string DequotedStr(string aStr, char aQuote = '"', bool aOnlyDoubleQuote = true)
            {
                // Проверка необходимости обработки строки
                if (string.IsNullOrEmpty(aStr))
                    return aStr;
                // Обработка строки
                StringBuilder tmpStr = new StringBuilder(aStr);
                DequotedStr(tmpStr, aQuote, aOnlyDoubleQuote);
                return tmpStr.ToString();
            }
            #endregion

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="aCommandLine">Командная строка</param>
            internal CmdParser(string aCommandLine)
            {
                CommandLine = aCommandLine;
                fUnquotedPath = !CommandLine.StartsWith(ccQuote.ToString());

                // Обход всех символов строки
                while (!EndOfText())
                {
                    // Обновим позицию старта
                    fStart = fPosition;
                    // Обработаем параметры в кавычках либо разделенные пробелами
                    if (Current() == ccQuote)
                        ParseBlock(MoveAhead());
                    else
                        ParseBlock(ccSpace);
                }
            }

            /// <summary>
            /// Поиск указанного ключа в параметрах
            /// </summary>
            /// <param name="aName">Имя ключа</param>
            /// <returns>Наличие ключа в параметрах</returns>
            public bool Exists(string aName)
            {
                foreach (CmdParserItem tmpFlag in Flags)
                    if (tmpFlag.Key.Equals(aName, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                return false;
            }

            /// <summary>
            /// Получить значение параметра
            /// </summary>
            public string ParamValue(string aName)
            {
                foreach (CmdParserItem tmpFlag in Flags)
                    if (tmpFlag.Key.Equals(aName, StringComparison.CurrentCultureIgnoreCase))
                        return DequotedStr(tmpFlag.Value);
                return string.Empty;
            }

            /// <summary>
            /// Список значений командной строки
            /// </summary>
            public List<CmdParserItem> Flags { get; } = new List<CmdParserItem>();

            /// <summary>
            /// Текущая командная строка строкой
            /// </summary>
            public string CommandLine { get; }

            /// <summary>
            /// Имя исполняемого файла
            /// </summary>
            public string FileName { get; private set; }
        }

        /// <summary>
        /// Класс описывающий пару ключ-значение
        /// </summary>
        private class CmdParserItem
        {
            /// <summary>
            /// Ключ
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Значение
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// Парсер для командной строки приложения
        /// </summary>
        private static CmdParser Parser { get; }

        /// <summary>
        /// Статический конструктор для инициализации общих данных
        /// </summary>
        static CommonCmd()
        {
            Parser = new CmdParser(Environment.CommandLine);
        }

        /// <summary>
        /// Поиск указанного ключа в параметрах
        /// </summary>
        /// <param name="aKey">Тип ключа</param>
        /// <returns>Признак наличия ключа</returns>
        public static bool Exists(object aKey)
        {
            return Parser.Exists(aKey.ToString());
        }

        /// <summary>
        /// Поиск указанного ключа в параметрах
        /// </summary>
        /// <param name="aKey">Тип ключа</param>
        /// <returns>Значение ключа</returns>
        public static string AsString(object aKey)
        {
            return Parser.ParamValue(aKey.ToString());
        }

        /// <summary>
        /// Поиск указанного ключа в параметрах
        /// </summary>
        /// <param name="aKey">Тип ключа</param>
        /// <returns>Значение ключа</returns>
        public static int AsInt(object aKey)
        {
            return int.Parse(Parser.ParamValue(aKey.ToString()));
        }
    }
}