using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this test class generation galaxy
// клас для визуализации генерации в юнити
public class Test : MonoBehaviour {

    //Префабы для тестирования (Инициализируется в юнити)
    public GameObject TestPlan;
	public GameObject TestSector;
	public GameObject TestSectorFull;
	public GameObject TestSectorbad;

    //полотно родитель для всех инстансов (Инициализируется в юнити)
    public GameObject canvas;

    //bool готовности сгенерированого созвездия
    public bool avake = false;

    //статическая ссылка на даный клас, используется для быстрого перехода к включеному екземпляру класа 
	public static Test test;
	
    //масив секторов для визуализации.
	public Sector[] sectors;

    //обект блокировки для синхронизации потоков юнити
	public  object locer = new object ();

    //инициализация ссылки
	void Start(){
		test = this;
	}

     //создание сектора
	void greateSectors(Sector sector){
		if(sector.bad){
			GameObject sec = Instantiate (TestSectorbad);
			sec.transform.position = new Vector3 (sector.posX, sector.posY, 0);
			sec.transform.parent = canvas.transform;
		}else{
		if (sector.full) {
			GameObject sec = Instantiate (TestSectorFull);
			sec.transform.position = new Vector3 (sector.posX, sector.posY, 0);
			sec.transform.parent = canvas.transform;
		} else {
			GameObject sec = Instantiate (TestSector);
			sec.transform.position = new Vector3 (sector.posX, sector.posY, 0);
			sec.transform.parent = canvas.transform;
		}
		}
	}

    //поочередное создание всех секторов
	void greateAllSector(){
		for(int i =0; i<sectors.Length; i++){
			greateSectors (sectors[i]);
		}
		Debug.Log ("Finish");
		avake = false;
		}

    //Создание планет внутри сектора
	void greatePlanetsSector(Sector sector){
		for(int i =0; i<sector.planets.Length;i++){
		GameObject plan = Instantiate (TestPlan);
			plan.transform.position = new Vector3 (sector.planets[i].posX,sector.planets[i].posY, 10);
		plan.transform.parent = canvas.transform;
		}
	}

    //Создание планет для каждого сектора по-очередно
	void GreatePlanetsAllSectors(){
		int k = 0;
		for(int i =0;i<sectors.Length;i++){
			greatePlanetsSector (sectors[i]);

		}
		sectors [0].DebugNumsPlanet ();
		//Debug.Log(k);
	}

    //В методе рендера идет проверка на готовность построеной галактики 
    //при готовности начинается процес визуализации сгенерированого созвездия
	void Update () {
		if(avake){
			lock (locer) {
				greateAllSector ();
				GreatePlanetsAllSectors ();
				avake = false;
			}
		}
	}
}
