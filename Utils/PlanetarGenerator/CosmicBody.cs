
namespace Empire.PlanetarGenerator
{
    /// <summary>
    /// Типы тел
    /// </summary>
    public enum BodyType
    {
        /// <summary>
        /// Маленькая
        /// </summary>
        Small,
        /// <summary>
        /// Обитаемая
        /// </summary>
        Big,
        /// <summary>
        /// Звезда
        /// </summary>
        Sun,
        /// <summary>
        /// Гидросостав
        /// </summary>
        Hydro,
        /// <summary>
        /// Карлик
        /// </summary>
        Rock,
        /// <summary>
        /// Черная дыра
        /// </summary>
        Hole,
        /// <summary>
        /// Пульсар
        /// </summary>
        Pulsar,
        /// <summary>
        /// Радужный мир
        /// </summary>
        Raindow,
        /// <summary>
        /// Не определенная
        /// </summary>
        Emty


    }

    /// <summary>
    ///  типы ресурсов
    /// </summary>
    public enum Resurse
    {
        /// <summary>
        /// без ресурсов
        /// </summary>
        Empty,
        /// <summary>
        /// Водород
        /// </summary>
        Vodorod,
        /// <summary>
        /// ксенон
        /// </summary>
        Xenon,
        /// <summary>
        /// Титан
        /// </summary>
        Titan,
        /// <summary>
        /// Кремний
        /// </summary>
        Kremniy,
        /// <summary>
        /// Антикристалы
        /// </summary>
        Antikristals
    }

    //Клас космическое тело 
    public class CosmicBody
    {

        //Кординаты тела по X и Y.
        public int posX, posY;
        //Тип планеты
        public BodyType bodyType;
        //Тип ресурсов на планете
        public Resurse resurse;
        //



        //конструктор создания пустого космического тела
        public CosmicBody(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
        }

        //конструктор создания тела с зарания извесным типом к примеру пульсары.
        public CosmicBody(int posX, int posY, BodyType bodyType)
        {
            this.posX = posX;
            this.posY = posY;
            this.bodyType = bodyType;
        }

    }


}