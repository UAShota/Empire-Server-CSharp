/////////////////////////////////////////////////
//
// Упаковка буфера и его потокобезопасная очередь
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using System.IO;

namespace Empire.Sockets
{
    /// <summary>
    /// Буфер бинарного протокола
    /// </summary>
    internal class SocketPacket : IDisposable
    {
        /// <summary>
        /// Буфер данных
        /// </summary>
        private MemoryStream fStream { get; set; }

        /// <summary>
        /// Читатель
        /// </summary>
        private BinaryReader fReader { get; set; }

        /// <summary>
        /// Писатель
        /// </summary>
        private BinaryWriter fWriter { get; set; }

        /// <summary>
        /// Позиция записи последней команды для записи итогового размера буфера
        /// </summary>
        private int fCommandPos { get; set; }

        /// <summary>
        /// Клиентское соединение
        /// </summary>
        public SocketConnection Connection { get; set; }

        /// <summary>
        /// Позиция указателя в буфере
        /// </summary>
        public int Position => (int)fStream.Position;

        /// <summary>
        /// Доступная длина буфера
        /// </summary>
        public int Length => (int)fStream.Length;

        /// <summary>
        /// Буфер данных
        /// </summary>
        public byte[] Buffer => fStream.GetBuffer();

        /// <summary>
        /// Инициализация переменных
        /// </summary>
        private void Init()
        {
            fStream = new MemoryStream();
            fReader = new BinaryReader(fStream);
            fWriter = new BinaryWriter(fStream);
        }

        /// <summary>
        /// Конструктор входящих буферов
        /// </summary>
        /// <param name="aConnection">Соединение для буфера</param>
        public SocketPacket(SocketConnection aConnection)
        {
            Init();
            Connection = aConnection;
        }

        /// <summary>
        /// Конструктор исходящих буферов
        /// </summary>
        /// <param name="aCommand">Команда инициализации</param>
        public SocketPacket(object aCommand)
        {
            Init();
            WriteCommand(aCommand);
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
            fReader.Dispose();
            fWriter.Dispose();
            fStream.Dispose();
        }

        /// <summary>
        /// Возвращение кода команды со сдвигом указателя
        /// </summary>
        /// <returns>Код команды</returns>
        public int ReadCommand()
        {
            fReader.BaseStream.Position = 0;
            return fReader.ReadInt32();
        }

        /// <summary>
        /// Чтение числа
        /// </summary>
        /// <returns>Число</returns>
        public int ReadInt()
        {
            return fReader.ReadInt32();
        }

        /// <summary>
        /// Чтение сериализованной строки
        /// </summary>
        /// <returns>Строка</returns>
        public string ReadString()
        {
            // Считаем длину строки        
            int tmpLen = ReadInt();
            // Скопируем
            return new string(fReader.ReadChars(tmpLen), 0, tmpLen);
        }

        /// <summary>
        /// Запись пустого блока под размер команды и запись команды
        /// </summary>
        /// <param name="aValue">Enum команда</param>
        public void WriteCommand(object aValue)
        {
            fCommandPos = Position;
            WriteInt(0);
            WriteInt(aValue);
        }

        /// <summary>
        /// Запись числа
        /// </summary>
        /// <param name="aValue">Число</param>
        public void WriteInt(int aValue)
        {
            fWriter.Write(aValue);
        }

        /// <summary>
        /// Запись Enum числа
        /// </summary>
        /// <param name="aValue">Enum число</param>
        public void WriteInt(object aValue)
        {
            fWriter.Write((int)aValue);
        }

        /// <summary>
        /// Запись булина
        /// </summary>
        /// <param name="aValue">Boolean значение</param>
        public void WriteBool(bool aValue)
        {
            fWriter.Write(aValue);
        }

        /// <summary>
        /// Запись блока из длины строки и самой строки
        /// </summary>
        /// <param name="aValue">Строка</param>
        public void WriteString(string aValue)
        {
            int tmpLength = aValue.Length;
            fWriter.Write(tmpLength);
            fWriter.Write(aValue.ToCharArray());
        }

        /// <summary>
        /// Запись буфера данных
        /// </summary>
        /// <param name="aBuffer">Буфер</param>
        /// <param name="aOffset">Смещение в буфере</param>
        /// <param name="aLength">Длина записываемого буфера</param>
        public void WriteBuffer(ref byte[] aBuffer, int aOffset, int aLength)
        {
            fStream.Write(aBuffer, aOffset, aLength);
        }

        /// <summary>
        /// Запись в буфер размера пакета без учета команды
        /// </summary>
        public void Commit()
        {
            fStream.Position = fCommandPos;
            fWriter.Write(Length - sizeof(int) - fCommandPos);
            fStream.Position = Length;
        }

        /// <summary>
        /// Подготовка буфера к работе
        /// </summary>
        public int Reset(ref byte[] aBuffer)
        {
            fStream.SetLength(0);
            fStream.Write(aBuffer, 0, sizeof(int));
            int tmpSize = ReadCommand();
            fStream.SetLength(0);
            return tmpSize;
        }
    }
}