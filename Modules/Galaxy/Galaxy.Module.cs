/////////////////////////////////////////////////
//
// Модуль обработки галактики
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.02.29
//
/////////////////////////////////////////////////

using Empire.Sockets;

namespace Empire.Modules
{
    /// <summary>
    /// Класс галактической обработки
    /// </summary>
    internal class GalaxyModule : CustomModule
    {
        /// <summary>
        /// Комманды
        /// </summary>
        private enum Commands
        {
            /// <summary>
            /// Подписка на созвездие
            /// </summary>
            Subscribe = 0x2F00
        }

        /// <summary>
        /// Подключение к планетарке
        /// </summary>
        /// <param name="aBuffer">Буфер команды</param>
        /// <returns>Запрет на уничтожение буфера</returns>
        private bool Subscribe(SocketPacket aBuffer)
        {
            return false;
        }

        /// <summary>
        /// Пересылка команды в созвездие
        /// </summary>
        /// <param name="aBuffer">Буфер данных</param>
        /// <returns>Запрет на уничтожение буфера</returns>
        private bool Retranslate(SocketPacket aBuffer)
        {
            return false;
        }

        /// <summary>
        /// Обработчик потока
        /// </summary>
        /// <param name="aBuffer">Буфер команды</param>
        protected override bool DoExecute(SocketPacket aBuffer)
        {
            // Определим команду
            Commands tmpCmd = (Commands)aBuffer.ReadCommand();
            // Определим команду
            switch (tmpCmd)
            {
                case Commands.Subscribe:
                    return Subscribe(aBuffer);
                default:
                    return Retranslate(aBuffer);
            }
        }
    }
}