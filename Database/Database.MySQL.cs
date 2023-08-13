/////////////////////////////////////////////////
//
// Модуль управления доступом к БД через MySQL
//
// Copyright(c) 2016 UAShota
//
// Rev I  2020.05.01
//
/////////////////////////////////////////////////

using System.Text;
using MySql.Data.MySqlClient;

namespace Empire.Database
{
    /// <summary>
    /// Класс доступа к данным MySQL
    /// </summary>
    internal class MySqlDatabaseReader : CustomDatabaseReader
    {
        /// <summary>
        /// Стандартная читалка
        /// </summary>
        private MySqlDataReader fReader { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aReader">Стандартная читалка</param>
        public MySqlDatabaseReader(MySqlDataReader aReader) : base()
        {
            fReader = aReader;
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public override void Dispose()
        {
            fReader.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Проверка на пустое значение
        /// </summary>
        /// <param name="aName">Имя поля</param>
        /// <param name="aIndex">Номер поля по имени</param>
        /// <returns>Признак пустого значения</returns>
        public override bool IsNull(string aName, out int aIndex)
        {
            aIndex = fReader.GetOrdinal(aName);
            return fReader.IsDBNull(aIndex);
        }

        /// <summary>
        /// Считывание следующий строки
        /// </summary>
        /// <returns>Наличие строки для считывания</returns>
        public override bool Read()
        {
            return fReader.Read();
        }

        /// <summary>
        /// Считывание числа
        /// </summary>
        /// <param name="aName">Имя поля</param>
        /// <returns>Число в БД</returns>
        public override int ReadInt(string aName)
        {
            if (IsNull(aName, out int aIndex))
                return 0;
            else
                return fReader.GetInt32(aIndex);
        }

        /// <summary>
        /// Считывание строки
        /// </summary>
        /// <param name="aName">Имя поля</param>
        /// <returns>Строка в БД</returns>
        public override string ReadString(string aName)
        {
            if (IsNull(aName, out int aIndex))
                return string.Empty;
            else
                return fReader.GetString(aIndex);
        }
    }

    /// <summary>
    /// Класс доступа к БД MySQL
    /// </summary>
    internal class MySqlDatabase : CustomDatabase
    {
        /// <summary>
        /// Строка соединения
        /// </summary>
        private const string csConnection = "server=localhost;database={0};user={1};password={2};";

        /// <summary>
        /// Экземпляр соединения
        /// </summary>
        private MySqlConnection fConnection { get; set; }

        /// <summary>
        /// Подключение к БД
        /// </summary>
        public MySqlDatabase(string aDatabase, string aUser, string aPassword) : base()
        {
            fConnection = new MySqlConnection(string.Format(csConnection, aDatabase, aUser, aPassword));
            fConnection.Open();
        }

        /// <summary>
        /// Отключение от БД
        /// </summary>
        public override void Dispose()
        {
            fConnection.Close();
            fConnection.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Сборка и выполнение запроса
        /// </summary>
        /// <param name="aCommand">Имя зранимой процедуры</param>*
        /// <param name="aParams">Массив параметров</param>
        /// <returns>Рекордсет запроса</returns>
        public override CustomDatabaseReader Query(string aCommand, params object[] aParams)
        {
            MySqlCommand tmpCommand = new MySqlCommand { Connection = (MySqlConnection)fConnection.Clone() };
            StringBuilder tmpBuilder = new StringBuilder(aCommand);
            // Соберем запрос и добавим параметры
            tmpBuilder.Append("(");
            for (int tmpI = 0; tmpI < aParams.Length; tmpI++)
            {
                tmpBuilder.Append("?");
                if (tmpI < aParams.Length - 1)
                    tmpBuilder.Append(",");
                tmpCommand.Parameters.Add(new MySqlParameter(tmpI.ToString(), aParams[tmpI]));
            }
            tmpBuilder.Append(")");
            // Откроем новое соединение
            tmpCommand.Connection.Open();
            tmpCommand.CommandText = tmpBuilder.ToString();
            // Выполним запрос
            return new MySqlDatabaseReader(tmpCommand.ExecuteReader());
        }
    }
}