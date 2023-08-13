/////////////////////////////////////////////////
//
// Добавление и удаление кораблика с планеты или созвездия
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс удаления кораблика с планеты или созвездия
    /// </summary>
    internal class ActionRelocation : PlanetaryAccess
    {
        /// <summary>
        /// Автозаполнение отряда при уничтожении кораблика
        /// </summary>
        /// <param name="aShip">Убиваемый кораблик</param>
        private void ReplaceDeleted(Ship aShip)
        {
            // Нельзя заменять нижний слот
            if (aShip.Landing.IsLowOrbit)
                return;
            // Нельзя заменять стационарки
            if (aShip.TechActive(ShipTech.Stationary))
                return;
            // Нельзя заменять артилерию
            if (aShip.TechActive(ShipTech.WeaponRocket))
                return;
            // Переберем все привязанные корабли
            foreach (Ship tmpShip in aShip.Landing.Planet.RangeAttackers)
            {
                if (tmpShip.ShipType != aShip.ShipType)
                    continue;
                if (tmpShip.Owner != aShip.Owner)
                    continue;
                // Проверим успешность вылета
                if (!Move(tmpShip, aShip.Landing, true))
                    continue;
                // Установим режим от уничтоженного кораблика
                if (aShip.Mode == ShipMode.Offline)
                {
                    tmpShip.Mode = aShip.Mode;
                    Engine.SocketWriter.ShipUpdateState(tmpShip);
                }
                break;
            }
        }

        /// <summary>
        /// Попытка найти планету для кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Успех перемещения</returns>
        private bool EscapeShip(Ship aShip)
        {
            // Сразу проверим топливо
            if (aShip.Fuel == 0)
                return false;
            // Перебираем планеты для отправки
            foreach (Planet tmpPlanet in aShip.Landing.Planet.Links)
            {
                if (Move(tmpPlanet, aShip, true, false))
                    return true;
            }
            // Не нашлось места
            return false;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionRelocation(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Добавление кораблика на планету
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aLanding">Слот посадки</param>
        /// <param name="aChangeState">Разрешить автосмену состояния</param>
        /// <param name="aCreate">Отправка сообщения создания кораблика</param>
        public void Add(Ship aShip, Landing aLanding, bool aChangeState, bool aCreate)
        {
            bool tmpChangeCount = (aShip.Landing == null) || (aShip.Planet != aLanding.Planet);
            aShip.Landing = aLanding;
            // Обновление параметров планеты
            if (tmpChangeCount)
                aShip.Planet.Ships.Add(aShip);
            // Если кораблик создается - отправим сообщение
            if (aCreate)
                Engine.SocketWriter.ShipCreate(aShip);
            // При загрузке с БД нельзя менять состояние
            Engine.Ships.Action.StandUp.Call(aShip, aChangeState, aChangeState, tmpChangeCount);
        }

        /// <summary>
        /// Удаление кораблика с планеты
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aChangePlanet">Признак слета корабля с планеты</param>
        public void Remove(Ship aShip, bool aChangePlanet)
        {
            // Уберем с слота посадки
            aShip.Landing.Ship = null;
            // Если кораблик улетает с планеты
            if (aChangePlanet)
            {
                aShip.Landing.Planet.Ships.Remove(aShip);
                Engine.Ships.Action.Attach.Call(aShip, null, true);
            }
            // Подчистим список активных
            Engine.Ships.Action.StandDown.Call(aShip, aShip.Mode, aChangePlanet);
        }

        /// <summary>
        /// Физическое удаление корабля с созвездия
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aExplose">Подрыв кораблика</param>
        public void Delete(Ship aShip, ShipDestroyMode aDestroyMode = ShipDestroyMode.None)
        {
            aShip.DestroyMode = aDestroyMode;
            // Уберем кораблик с планеты
            Remove(aShip, true);
            // Удалим из группы
            /*GroupRemove(aShip);*/
            // Уберем все таймеры
            Engine.Timers.RemoveAll(aShip);
            // Отправим сообщение
            Engine.SocketWriter.ShipDelete(aShip);
            // Заменим кораблик, если было уничтожение
            if (aDestroyMode == ShipDestroyMode.Explose)
                ReplaceDeleted(aShip);
        }

        /// <summary>
        /// Раскидывание кораблей с планеты
        /// </summary>
        /// <param name="aPlanet">Обрабатываемая планета</param>
        public void Escape(Planet aPlanet)
        {
            // Попробуем раскидать все корабли
            for (int tmpI = aPlanet.Ships.Count - 1; tmpI >= 0; tmpI--)
            {
                Ship tmpShip = aPlanet.Ships[tmpI];
                // Выключим порталинг
                Engine.Ships.Action.Utils.TimerRemove(tmpShip, ShipTimer.PortalJump);
                // Попробуем принудительно отправить на другую планету только если есть топливо
                if (!EscapeShip(tmpShip))
                    Delete(tmpShip, ShipDestroyMode.Explose);
            }
        }

        /// <summary>
        /// Прыжок между планетами (например между черными дырами)
        /// </summary>
        /// <param name="aShip"></param>
        /// <param name="aPlanet"></param>
        /// <returns>Успех перемещения</returns>
        public bool Jump(Ship aShip, Planet aPlanet)
        {
            // Проверим на переполнение
            if (!Engine.Ships.Action.Utils.CheckArrival(aPlanet, aShip.Owner, true))
                return false;
            // Поищем свободный слот
            if (!Engine.Ships.Action.Utils.GetSlot(aPlanet, aShip, true, out Landing tmpLanding))
                return false;
            // Удалим с текущей планеты
            Remove(aShip, true);
            // Отправим сообщение, на сервере сменится ID корабля
            Engine.SocketWriter.ShipJumpTo(aShip, tmpLanding);
            // Добавим на новую
            Add(aShip, tmpLanding, true, false);
            // Вернем успех операции
            return true;
        }

        /// <summary>
        /// Перемещение корабля на указанный слот
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aLanding">Слот прилета</param>
        /// <param name="aAutoAttach">Необходимость автопривязки после перелета</param>
        /// <returns>Успех перемещения</returns>
        public bool Move(Ship aShip, Landing aLanding, bool aAutoAttach)
        {
            // Проверим возможность слета
            if (!Engine.Ships.Action.Utils.CheckDeparture(aShip, aLanding.Planet))
                return false;
            // Проверим на возможность прилета
            if ((aShip.Landing.Planet != aLanding.Planet) && !Engine.Ships.Action.Utils.CheckArrival(aLanding.Planet, aShip.Owner, false))
                return false;
            // Должна быть активна теха пролета в тыл или назначение не тыл
            if (!Engine.Ships.Action.Utils.CheckBackZone(aLanding, aShip.Owner) && !aShip.TechActive(ShipTech.BackzoneIntruder))
                return false;
            // Удалить с текущей планеты
            Remove(aShip, aShip.Planet != aLanding.Planet);
            // Установка времени перелета
            if (aShip.Planet == aLanding.Planet)
                Engine.Ships.Action.Fly.Call(aShip, ShipFlyType.Local);
            else
                Engine.Ships.Action.Fly.Call(aShip, ShipFlyType.Global);
            // Отправим сообщение, на сервере сменится ID корабля
            Engine.SocketWriter.ShipMoveTo(aShip, aLanding);
            // Добавить на планету назначения
            Add(aShip, aLanding, false, false);
            // Приаттачим если нужно
            if (aAutoAttach)
                Engine.Ships.Action.Attach.Call(aShip, aShip.Planet, false);
            // Вернем успех операции
            return true;
        }

        /// <summary>
        /// Перемещение корабля на свободный слот
        /// </summary>
        /// <param name="aPlanet">Планета назначения</param>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aRandomSlot">Первый или случайный слот</param>
        /// <param name="aAutoAttach">Привязать корабль после перемещения</param>
        /// <returns></returns>
        public bool Move(Planet aPlanet, Ship aShip, bool aRandomSlot, bool aAutoAttach)
        {
            if (!Engine.Ships.Action.Utils.GetSlot(aPlanet, aShip, aRandomSlot, out Landing tmpLanding))
                return false;
            else
                return Move(aShip, tmpLanding, aAutoAttach);
        }
    }
}