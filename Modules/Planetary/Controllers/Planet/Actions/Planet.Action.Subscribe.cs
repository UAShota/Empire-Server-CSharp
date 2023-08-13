/////////////////////////////////////////////////
//
// Подписка на планету
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2019.06.01
//
/////////////////////////////////////////////////

using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс организации подписки на планету
    /// </summary>
    internal class Subscribe : PlanetaryAccess
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public Subscribe(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Подписка на планету
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        public void Call(Planet aPlanet, Player aPlayer)
        {
            // Нельзя подписаться на неактивную
            if (aPlanet.State == PlanetState.Inactive)
            {
                Core.Log.Warn("Planet inactive");
                return;
            }
            // Нельзя подписаться на уже подписанную
            /*if (not TPlanetThread(Engine).SocketWriter.PlanetSubscribe(APlayer, aPlanet))*/
            // Нельзя подписаться на невидимую игроку планету
            if (!aPlanet.VisibleByPlayer(aPlayer, false))
            {
                Core.Log.Warn("Not visible");
                return;
            }
            // Отправим сообщение что игрок подписан
            Engine.SocketWriter.PlanetSubscriptionChange(aPlanet, true, aPlayer);
            // Отправим сообщение о текущем состоянии
            Engine.SocketWriter.PlanetStateUpdate(aPlanet, aPlayer);
            // Отправим сообщение о владельце
            Engine.SocketWriter.PlanetOwnerChanged(aPlanet, aPlayer);
            // Отправим таймеры
            foreach (Timer tmpTimer in aPlanet.Timers)
                if (tmpTimer != null)
                    Engine.SocketWriter.PlanetUpdateTimer(tmpTimer);
            // Отправим пользователю данные о флоте, чтобы потом можно было выбрать цель
            foreach (Ship tmpShip in aPlanet.Ships)
                Engine.SocketWriter.ShipCreate(tmpShip, aPlayer);
            // Отправим прицеливание корабликов
            foreach (Ship tmpShip in aPlanet.Ships)
            {
                /*for TmpWeapon := Low(TPlShipWeaponType) to High(TPlShipWeaponType) do
                    {
                        if (Assigned(TmpShip.Targets[TmpWeapon])) then
                    TPlanetThread(Engine).SocketWriter.ShipRetarget(TmpShip, TmpWeapon, APlayer);
                    }*/
            }
            // Отправим торговые пути
            /*if (Assigned(aPlanet.ResPathOut)) then
              TPlanetThread(Engine).SocketWriter.PlanetTradePathUpdate(aPlanet, APlayer);*/
            // Отправим сообщение о портале
            if (aPlanet.Portal != null)
                Engine.SocketWriter.PlanetPortalOpen(aPlanet.Portal, aPlayer);
        }
    }
}