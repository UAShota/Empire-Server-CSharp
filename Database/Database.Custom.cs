/////////////////////////////////////////////////
//
// Модуль управления доступом к БД
//
// Copyright(c) 2016 UAShota
//
// Rev I  2020.05.01
//
/////////////////////////////////////////////////

using Empire.EngineSpace;

namespace Empire.Database
{
    /// <summary>
    /// Класс доступа к данным
    /// </summary>
    internal abstract class CustomDatabaseReader : Leak
    {
        /// <summary>
        /// Проверка на пустое значение
        /// </summary>
        /// <param name="aName">Имя поля</param>
        /// <param name="aIndex">Номер поля по имени</param>
        /// <returns>Признак пустого значения</returns>
        public abstract bool IsNull(string aName, out int aIndex);

        /// <summary>
        /// Считывание следующий строки
        /// </summary>
        /// <returns>Наличие строки для считывания</returns>
        public abstract bool Read();

        /// <summary>
        /// Считывание числа
        /// </summary>
        /// <param name="aName">Имя поля</param>
        /// <returns>Число в БД</returns>
        public abstract int ReadInt(string aName);

        /// <summary>
        /// Считывание строки
        /// </summary>
        /// <param name="aName">Имя поля</param>
        /// <returns>Строка в БД</returns>
        public abstract string ReadString(string aName);
    }

    /// <summary>
    /// Класс доступа к БД
    /// </summary>
    internal abstract class CustomDatabase : Leak
    {
        /// <summary>
        /// Сборка и выполнение запроса
        /// </summary>
        /// <param name="aCommand">Имя зранимой процедуры</param>
        /// <param name="aParams">Массив параметров</param>
        /// <returns>Рекордсет запроса</returns>
        public abstract CustomDatabaseReader Query(string aCommand, params object[] aParams);
    }
}