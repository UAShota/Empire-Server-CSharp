/////////////////////////////////////////////////
//
// Базовый класс для планетарных протоколов
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.02.29
//
/////////////////////////////////////////////////

using System.Collections.Concurrent;
using Empire.EngineSpace;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.Protocol
{
    internal class ProtocolReader : CustomProtocol
    {
        #region Переменные

        /// <summary>
        /// Команды клиента
        /// </summary>
        private enum Commands
        {
            /// <summary>
            /// Подписка на созвездие
            /// </summary>
            PlanetarySubscribe = 0x1F02,
            /// <summary>
            /// Объединение кораблей
            /// </summary>
            ShipMerge = 0x1000,
            /// <summary>
            /// Перемещение корабля
            /// </summary>
            ShipMoveTo = 0x1001,
            /// <summary>
            /// Аттач корабля к планете
            /// </summary>
            ShipAttachTo = 0x1002,
            /// <summary>
            /// Установка торгового пути планеты
            /// </summary>
            PlanetTradePath = 0x1003,
            /// <summary>
            /// Перемещение корабля с ангара
            /// </summary>
            ShipFromHangar = 0x1004,
            /// <summary>
            /// Подписка на планету
            /// </summary>
            PlanetSubscribe = 0x1005,
            /// <summary>
            /// Перемещение корабля в ангар
            /// </summary>
            ShipToHangar = 0x1006,
            /// <summary>
            /// Показ деталей планеты
            /// </summary>
            PlanetShowDetails = 0x1007,
            /// <summary>
            /// Перемещение ресурса между слотами
            /// </summary>
            ResourceMove = 0x1008,
            /// <summary>
            /// Постройка корабля
            /// </summary>
            ShipConstruction = 0x1009,
            /// <summary>
            /// Покупка технологии корабля
            /// </summary>
            ShipTechBuy = 0x100A,
            /// <summary>
            /// Постройка здания
            /// </summary>
            BuildingConstruct = 0x100B,
            /// <summary>
            /// Покупка технологии строения
            /// </summary>
            BuildingTechBuy = 0x100C,
            /// <summary>
            /// Смена состояния корабля
            /// </summary>
            ShipChangeActive = 0x100D,
            /// <summary>
            /// Усреднение количества кораблей в стеках
            /// </summary>
            ShipHypodispersion = 0x100E,
            /// <summary>
            /// Перемещение корабля в группу
            /// </summary>
            ShipMoveToGroup = 0x100F,
            /// <summary>
            /// Разделение стеков
            /// </summary>
            ShipSeparate = 0x1010,

            /**/
            HangarLock = 0x1011,

            /// <summary>
            /// Открытия портала
            /// </summary>
            ShipPortalOpen = 0x1012,
            /// <summary>
            /// Уничтожение корабля
            /// </summary>
            ShipDestroy = 0x1013,
            /// <summary>
            /// Закрытие портала
            /// </summary>
            PlanetPortalClose = 0x1014,
            /// <summary>
            /// Прыжок в портал
            /// </summary>
            ShipPortalJump = 0x1015,
            /// <summary>
            /// Запуск аннигиляции
            /// </summary>
            ShipAnnihilation = 0x1016,
            /// <summary>
            /// Использования скилла конструктора
            /// </summary>
            ShipSkillConstructor = 0x1017,
            /// <summary>
            /// Перемещение кораблей в слоте ангара
            /// </summary>
            ShipHangarSwap = 0x1018
        }

        /// <summary>
        /// Количество обрабатываемых команд в планетарный тик
        /// </summary>
        private const int ciMaxCommandsPerTick = 100;

        /// <summary>
        /// Очередь буфера команд
        /// </summary>
        private readonly ConcurrentQueue<SocketPacket> fQueue = new ConcurrentQueue<SocketPacket>();

        #endregion

        #region Обработка

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aCore">Движок планетарки</param>
        public ProtocolReader(PlanetaryEngine aCore) : base(aCore)
        {
        }

        /// <summary>
        /// Добавление команды в стек команд
        /// </summary>
        /// <param name="aBuffer">Буфер данных</param>
        public void Command(SocketPacket aBuffer)
        {
            fQueue.Enqueue(aBuffer);
        }

        /// <summary>
        /// Обработка очереди буферов команд
        /// </summary>
        public void Work()
        {
            int tmpMaxCount = ciMaxCommandsPerTick;
            // Поищем буфер для работы
            while (tmpMaxCount-- > 0)
            {
                if (!fQueue.TryDequeue(out fPacket))
                    return;
                // Определим команду
                Commands tmpCommand = (Commands)fPacket.ReadCommand();
                // Проверим команду на исполнение            
                switch (tmpCommand)
                {
                    case Commands.PlanetarySubscribe:
                        PlanetarySubscribe();
                        break;
                    case Commands.ShipMerge:
                        Engine.Ships.Cmd.Merge.Process(fPacket);
                        break;
                    case Commands.ShipSeparate:
                        Engine.Ships.Cmd.Separate.Process(fPacket);
                        break;
                    case Commands.ShipMoveTo:
                        Engine.Ships.Cmd.Move.Process(fPacket);
                        break;
                    case Commands.ShipMoveToGroup:
                        Engine.Ships.Cmd.Path.Process(fPacket);
                        break;
                    case Commands.ShipAttachTo:
                        Engine.Ships.Cmd.Attach.Process(fPacket);
                        break;
                    case Commands.PlanetTradePath:
                        PlanetTradePath();
                        break;
                    case Commands.ShipFromHangar:
                        Engine.Hangar.Cmd.Departure.Process(fPacket);
                        break;
                    case Commands.ShipToHangar:
                        Engine.Hangar.Cmd.Arrival.Process(fPacket);
                        break;
                    case Commands.ShipConstruction:
                        Engine.Ships.Cmd.Construction.Process(fPacket);
                        break;
                    case Commands.ShipChangeActive:
                        Engine.Ships.Cmd.Activity.Process(fPacket);
                        break;
                    case Commands.ShipHypodispersion:
                        Engine.Ships.Cmd.Hypodispersion.Process(fPacket);
                        break;
                    case Commands.PlanetSubscribe:
                        Engine.Planets.Cmd.Subscribe.Process(fPacket);
                        break;
                    case Commands.PlanetShowDetails:
                        Engine.Planets.Cmd.ShowDetails.Process(fPacket);
                        break;
                    case Commands.ResourceMove:
                        ResourceMove();
                        break;
                    case Commands.ShipTechBuy:
                        ShipTechBuy();
                        break;
                    case Commands.BuildingTechBuy:
                        BuildingTechBuy();
                        break;
                    case Commands.BuildingConstruct:
                        BuildingConstruct();
                        break;
                    case Commands.ShipPortalOpen:
                        Engine.Ships.Cmd.PortalOpen.Process(fPacket);
                        break;
                    case Commands.PlanetPortalClose:
                        Engine.Planets.Cmd.PortalClose.Process(fPacket);
                        break;
                    case Commands.ShipPortalJump:
                        Engine.Ships.Cmd.PortalJump.Process(fPacket);
                        break;
                    case Commands.ShipDestroy:
                        Engine.Ships.Cmd.Destroy.Process(fPacket);
                        break;
                    case Commands.ShipAnnihilation:
                        Engine.Ships.Cmd.Annihilation.Process(fPacket);
                        break;
                    case Commands.ShipSkillConstructor:
                        Engine.Ships.Cmd.Constructor.Process(fPacket);
                        break;
                    case Commands.ShipHangarSwap:
                        Engine.Hangar.Cmd.Swap.Process(fPacket);
                        break;
                    case Commands.HangarLock:
                        HangarLock();
                        break;
                    default:
                        Core.Log.Warn("Invalid Planet reader command 0x{0:X}", tmpCommand);
                        break;
                }
                // Удалим буфер
                fPacket.Dispose();
            }
        }

        /**/
        private void HangarLock()
        {
            int UID = fPacket.ReadInt();
            /*Engine.ShipController.Cmd.Hangar.Lock(UID, fBuffer.Connection.Player);*/
        }

        #endregion

        #region Система

        /// <summary>
        /// Подписка на планетарку
        /// </summary>
        private void PlanetarySubscribe()
        {
            Engine.Subscribe(fPacket.Connection.Player);
        }

        #endregion

        #region Планеты

        private void PlanetTradePath()
        {
        }

        #endregion

        private void ResourceMove()
        {
        }

        private void BuildingConstruct()
        {
        }

        private void ShipTechBuy()
        {
            ShipType tmpType = (ShipType)fPacket.ReadInt();
            ShipTech tmpTech = (ShipTech)fPacket.ReadInt();
            // Отправим команду на исполнение
            Engine.Player.Planetary.BuyTech(tmpType, tmpTech);
        }

        private void BuildingTechBuy()
        {
        }
    }
}