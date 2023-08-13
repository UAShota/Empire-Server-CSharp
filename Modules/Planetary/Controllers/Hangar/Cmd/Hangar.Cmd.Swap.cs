/////////////////////////////////////////////////
//
// Команда игрока - замена слотов ангара
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.HangarSpace
{
    /// <summary>
    /// Класс замены слотов ангара
    /// </summary>
    internal class CmdSwap : PlanetaryCommand
    {
        /// <summary>
        /// Обмен слотов и уведомление игрока
        /// </summary>
        /// <param name="aPlayer">Инициирующий игрок</param>
        /// <param name="aSource">Ангар источник</param>
        /// <param name="aTarget">Ангар назначение</param>
        private void Call(Player aPlayer, Hangar aSource, Hangar aTarget)
        {
            // Обменяем
            aPlayer.Planetary.HangarZone.Swap(aSource, aTarget);
            // Обновим
            Engine.SocketWriter.PlayerHangarUpdate(aSource, aPlayer);
            Engine.SocketWriter.PlayerHangarUpdate(aTarget, aPlayer);
        }

        /// <summary>
        /// Проверка слота источника
        /// </summary>
        /// <param name="aPlayer">Владелец ангара</param>
        /// <param name="aPoition">Позиция слота</param>
        /// <param name="aHangar">Найденный объект ангара</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTarget(Player aPlayer, int aPoition, out Hangar aHangar)
        {
            if (!aPlayer.Planetary.HangarZone.TryGetSlot(aPoition, out aHangar))
                return Warning("Wrong target");
            else
                return true;
        }

        /// <summary>
        /// Проверка слота источника
        /// </summary>
        /// <param name="aPlayer">Владелец ангара</param>
        /// <param name="aPoition">Позиция слота</param>
        /// <param name="aHangar">Найденный объект ангара</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckSource(Player aPlayer, int aPoition, out Hangar aHangar)
        {
            if (!aPlayer.Planetary.HangarZone.TryGetSlot(aPoition, out aHangar))
                return Warning("Wrong source");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdSwap(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            int tmpSourcePosition = aPacket.ReadInt();
            int tmpTargetPosition = aPacket.ReadInt();
            Player tmpPlayer = aPacket.Connection.Player;
            // Проверки
            if (!CheckSource(tmpPlayer, tmpSourcePosition, out Hangar tmpSource))
                return;
            if (!CheckTarget(tmpPlayer, tmpTargetPosition, out Hangar tmpTarget))
                return;
            // Отправим команду на исполнение
            Call(tmpPlayer, tmpSource, tmpTarget);
        }
    }
}