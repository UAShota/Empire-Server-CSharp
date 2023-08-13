/////////////////////////////////////////////////
//
// Привязка кораблика к планете
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
    /// Класс привязки кораблика к планете
    /// </summary>
    internal class ActionAttach : PlanetaryAccess
    {
        /// <summary>
        /// Переприцеливание ранжевика или захват планеты захватчиком
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aAutoTarget">Призак автопривязки</param>
        private void Retarget(Ship aShip, bool aAutoTarget)
        {
            if (aAutoTarget)
                return;
            if (aShip.TechActive(ShipTech.WeaponRocket))
                Engine.Ships.Action.Battle.Call(aShip);
            else
                Engine.Planets.Action.Capture.Call(aShip);
        }

        /// <summary>
        /// Переприсоединение ранжевика, прикрепляется к планете либо летит на захват
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aAutoTarget">Признак автоприсоединения</param>
        private void Reattach(Ship aShip, bool aAutoTarget)
        {
            // Запишем в внешние корабли планеты
            aShip.Attach.RangeAttackers.Add(aShip);
            if (aAutoTarget)
                return;
            // И переприцелим вручную направленные девы
            if (aShip.TechActive(ShipTech.WeaponRocket))
            {
                Engine.Ships.Action.TargetMarker.Highlight(aShip, aShip.Attach, false);
                return;
            }
            // Инвайдер при таком прикреплении сразу летит захватывать
            if (!aShip.TechActive(ShipTech.Capturer))
                return;
            // Захватываем только планеты с населением
            if (aShip.Attach.Type != PlanetType.Earth)
                return;
            // Пытаемся перелететь на планету
            Engine.Ships.Action.Relocation.Move(aShip.Attach, aShip, true, true);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public ActionAttach(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Привязка кораблика к планете
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestination">Планета</param>
        /// <param name="aAutoTarget">Признак привязки через автопоиск цели</param>
        public void Call(Ship aShip, Planet aDestination, bool aAutoTarget)
        {
            // Если корабль не приаттачен то сразу выходим
            if ((aShip.Attach == null) && (aDestination == null))
                return;
            // Уберем кораблик из списка нацеленных
            if (aShip.IsRangeAttach)
                aShip.Attach.RangeAttackers.Remove(aShip);
            // Отправим сообщение о смене аттача
            aShip.IsAutoTarget = (aAutoTarget) && (aDestination != null);
            aShip.Attach = aDestination;
            Engine.SocketWriter.ShipChangeAttach(aShip);
            // Если приаттачены не к внешней планете
            if ((aDestination == null) || (aDestination == aShip.Planet))
                Retarget(aShip, aAutoTarget);
            else
                Reattach(aShip, aAutoTarget);
        }
    }
}