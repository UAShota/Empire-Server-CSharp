/////////////////////////////////////////////////
//
// Изменение параметров портала
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Управление порталами планетоида
    /// </summary>
    internal class Portals : PlanetaryAccess
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public Portals(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Открытие портала
        /// </summary>
        /// <param name="aSource">Планета источника</param>
        /// <param name="aTarget">Планета назначения</param>
        /// <param name="aOwner">Владелец портала</param>
        /// <param name="aLimit">Лимит корабликов на перелет</param>
        /// <param name="aBreakable">Возможность перебить портал</param>
        public void Open(Planet aSource, Planet aTarget, Player aOwner, int aLimit, bool aBreakable)
        {
            Engine.SocketWriter.PlanetPortalOpen(new Portal(aSource, aTarget, aOwner, aLimit, aBreakable), aOwner);
        }

        /// <summary>
        /// Открытие портала пользователем
        /// </summary>
        /// <param name="aShip">Корабль инициатор</param>
        /// <param name="aPlanet">Планета назначения</param>
        /// <param name="aPlayer">Управляющий игрок</param>
        public void Open(Ship aShip, Planet aPlanet, Player aPlayer)
        {
            // Нельзя кидать порталы чт
            if (aPlanet.Type == PlanetType.Hole)
            {
                Core.Log.Warn("Target hole");
                return;
            }
            // Нельзя кидать порталы чт
            if (aShip.Landing.Planet.Type == PlanetType.Hole)
            {
                Core.Log.Warn("Source hole");
                return;
            }
            // Нельзя кидать портал на самого себя
            if (aShip.Landing.Planet == aPlanet)
            {
                Core.Log.Warn("Himself");
                return;
            }
            // Нельзя открывать второй портал
            if (aShip.Landing.Planet.Portal != null)
            {
                Core.Log.Warn("Source double");
                return;
            }
            // Нельзя открывать второй портал
            if (aPlanet.Portal != null)
            {
                Core.Log.Warn("Target double");
                return;
            }
            // Нельзя открывать если нет техи открытия постоянных порталов
            if (!aShip.TechActive(ShipTech.StablePortal))
            {
                Core.Log.Warn("No stable portal tech");
                return;
            }
            // Нельзя открывать с чужого кораблика
            if (!aPlayer.IsRoleFriend(aShip.Owner))
            {
                Core.Log.Warn("No friend");
                return;
            }
            // Нельзя открывать если нет приемщика на другой планете
            /*    if (!CheckFriendlyPortaler(aPlanet, aShip.Owner))
                {
                    Core.Log.Warn("No reciever");
                    return;
                }*/
            // Построим планетарный портал
            Open(aShip.Landing.Planet, aPlanet, aShip.Owner, -1, aShip.ShipType == ShipType.Scient);
        }

        /// <summary>
        /// Закрытие портала на планете
        /// </summary>
        /// <param name="aPortal">Портал</param>
        /// <param name="aIn">Признак входа в портал</param>
        public void Close(Portal aPortal, bool aIn)
        {
            Planet tmpPlanet;
            // При первом проходе выключаем вход, потом выход
            if (aIn)
                tmpPlanet = aPortal.In;
            else
                tmpPlanet = aPortal.Out;
            // Отправим сообщение о закрытии портала между планетами
            Engine.SocketWriter.PlanetPortalClose(tmpPlanet);
            // Отменим порталинг корабликов
            foreach (Ship tmpShip in tmpPlanet.Ships)
            {
                if (tmpShip.TimerEnabled(ShipTimer.PortalJump))
                {
                    Engine.Ships.Action.Utils.TimerRemove(tmpShip, ShipTimer.PortalJump);
                    Engine.Ships.Action.StandUp.Call(tmpShip, true, false);
                }
            }
            // После закрытия выхода, удаляем портал
            if (aIn)
                Close(aPortal, false);
            else
                aPortal.Dispose();
        }

        /// <summary>
        /// Закрытие портала пользователем
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Управляющий игрок</param>
        public void Close(Planet aPlanet, Player aPlayer)
        {
            // Нельзя закрыть неоткрытый портал
            if (aPlanet.Portal == null)
            {
                Core.Log.Warn("No portal");
                return;
            }
            // Нельзя закрыть портал БЧТ
            if (aPlanet.Portal.Owner == null)
            {
                Core.Log.Warn("Owner");
                return;
            }
            // Нельзя закрыть вражеский портал
            if (!aPlanet.Portal.Owner.IsRoleFriend(aPlayer))
            {
                Core.Log.Warn("Enemy portal");
                return;
            }
            Close(aPlanet.Portal, true);
        }
    }
}