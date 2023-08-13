using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneratorGalaxy : MonoBehaviour {
	// class generation galaxy
	Configuration config;
	int numsSetors;
	System.Random r= new System.Random();
	int columSector;
	int lineSector;

	int c1enterColumAndLine;

	int GenerateLine=0;
	int GenerateColum=0;

	static Sector[] sectors;
	static Sector[,] sectorsTable;
	static Sector[] sectorsInternal = new Sector[0];
	static Sector[] sectorsAverag= new Sector[0];
	static Sector[] sectorsExternal= new Sector[0];
	static Sector[] sectorsBad = new Sector[0];
	static Sector[] sectorsNull = new Sector[0];
	static Sector[] sectorsIternalAveragPulsar = new Sector[0];
	static Sector[] sectorsAveragExternalPulsar = new Sector[0];
	static Sector[] sectorsPulsar1 = new Sector[0];
	static Sector[] sectorsPulsar2 = new Sector[0];


	int numGenerationSector =0;

	bool timeNighborinJudje =false; 
	bool generateSector = true;
	bool timeJudjeSector = false;
	bool timeKillCorountine = false;
	bool timeGreateNullSectors = false;
	bool timeWorkBadSectors = false;
	bool timeWorkPulsar = false;

	static bool coruntineCan = true;
	static object locker = new object();
	static object lockerSector = new object ();

	int centerX;
	int centerY;

	bool avaibleIternal = false;
	bool avaibleAverage = false;
	bool avaibleExternal = false;

	bool timeGreateConect = false;
	bool timeCreatePlanets = false;


	void Start(){
		config = new Configuration();
		DeterminNumberSectors ();
	}

	void DeterminNumberSectors(){
		columSector = (int)Math.Sqrt(config.maxPlanetGalaxy/config.advangBodyInOneSector)+r.Next(4,8);
		lineSector = columSector;
		numsSetors = columSector * lineSector;
		sectorsTable = new Sector[lineSector, columSector];
		sectors = new Sector[numsSetors];
//		Debug.Log (numsSetors);
	}

	IEnumerator Sector(){
		while (coruntineCan) {
//			Debug.Log ("this");
			AddSector ();
			yield return null;
		}
		yield return null;
	}

	bool AvaibleRangeSectors(Sector sector,Sector sector1,int minDistanse){
		double distance;
		distance = Math.Sqrt (((sector.posX - sector1.posX) * (sector.posX - sector1.posX))
		+ ((sector.posY - sector1.posY) * (sector.posY - sector1.posY)));
		if (distance < minDistanse) {
			return false;
		} else{
			return true;
	}
	}

	bool AvaibleItAv(Sector sector){
		bool result = true;
		for(int i = 0;i<sectorsIternalAveragPulsar.Length;i++){
			if(sectorsIternalAveragPulsar[i].greated){
				//Debug.Log ("12345");
				if (AvaibleRangeSectors (sector, sectorsIternalAveragPulsar [i],40)) {
					result = true;
					//sector.FreePointConectPulsar ();
				} else {
					return false;
				}

				
			}
		}
		return result;
	}

	bool AvaibleAvEx(Sector sector){
		bool result = true;
		for(int i = 0;i<sectorsAveragExternalPulsar.Length;i++){
			if(sectorsAveragExternalPulsar[i].greated){
			//	Debug.Log ("12345");
				if (AvaibleRangeSectors (sector, sectorsAveragExternalPulsar [i],90)) {
					result = true;
					//sector.FreePointConectPulsar ();
				} else {
					return false;
				}


			}
		}
		return result;
	}

	bool AddItAv(int attempt){
		if(attempt>20){
			return false;
		}
		int nums = r.Next (sectorsIternalAveragPulsar.Length - 1);
		if (AvaibleItAv (sectorsIternalAveragPulsar [nums])) {
			//sectorsIternalAveragPulsar [nums].bad = true;
			sectorsIternalAveragPulsar [nums].greated = true;
			AddSectors2Pulsar (sectorsIternalAveragPulsar [nums]);
			sectorsIternalAveragPulsar [nums].FreePointConectPulsar ();
			return true;
		//	Debug.Log ("trtdshgtrhnwr");
		} else {
			AddItAv (attempt + 1);
			Debug.Log ("trtdshgtrhnwr");
			return false;
		}

	}



	bool AddAvEx(int attempt){
		if(attempt>40){
			return false;
		}
		int nums = r.Next (sectorsAveragExternalPulsar.Length - 1);
		if (AvaibleAvEx (sectorsAveragExternalPulsar [nums])) {
			sectorsAveragExternalPulsar [nums].FreePointConectPulsar ();
			sectorsAveragExternalPulsar [nums].bad = true;
			sectorsAveragExternalPulsar [nums].greated = true;
			AddSectors1Pulsar (sectorsAveragExternalPulsar [nums]);
			return true;
			//	Debug.Log ("trtdshgtrhnwr");
		} else {
			AddAvEx (attempt + 1);
			return false;
		}

	}

	IEnumerator WorkPulsar(){
		InitializeNullSectors ();
		PulsarSectorsIternalAverageInitialize ();
		PulsarSectorsAverageExternalInitialize ();
		int numsPulsarItAv = r.Next(4,7);
		int numsPulAvEx = r.Next (4,9);
		for(int i = 0;i<numsPulsarItAv;i++){
			AddItAv (0);

		}
		for(int i = 0;i<numsPulAvEx;i++){
			AddAvEx (0);
		}
		Finish ();
		yield return null;
	}

	void InitializeNullSectors(){
		for(int i =0;i<sectors.Length;i++){
			if(sectors[i].typeSector<1){
				AddNullSector (sectors[i]);
			}
		}
	}

	void PulsarSectorsIternalAverageInitialize(){
		for(int i =0;i<sectorsNull.Length;i++){
			if(sectorsNull[i].FriendIternal()&&sectorsNull[i].FriendAverag()){
				AddIternalAveragSectrorsPulsar (sectorsNull[i]);
				}
			}
		}

	void PulsarSectorsAverageExternalInitialize(){
		for(int i =0;i<sectorsNull.Length;i++){
			if(sectorsNull[i].FriendExternal()&&sectorsNull[i].FriendAverag()){
				AddAveragExternalSectrorsPulsar (sectorsNull[i]);
			}
		}
	}

	void NextSector(){
		if (GenerateLine < lineSector-1) {
			GenerateLine++;
		} else {
			if (GenerateColum < columSector) {
				GenerateLine = 0;
				GenerateColum++;
			} 
		}
	
	}

	void AddIternalAveragSectrorsPulsar(Sector sector){
		Sector[] permission = sectorsIternalAveragPulsar;
		sectorsIternalAveragPulsar = new Sector[permission.Length+1];
		for(int i = 0; i<permission.Length;i++){
			sectorsIternalAveragPulsar[i] = permission[i];
		}
		sectorsIternalAveragPulsar [permission.Length] = sector;
	}

	void AddAveragExternalSectrorsPulsar(Sector sector){
		Sector[] permission = sectorsAveragExternalPulsar;
		sectorsAveragExternalPulsar = new Sector[permission.Length+1];
		for(int i = 0; i<permission.Length;i++){
			sectorsAveragExternalPulsar[i] = permission[i];
		}
		sectorsAveragExternalPulsar [permission.Length] = sector;
	}

	void AddSectors1Pulsar(Sector sector){
		Sector[] permission = sectorsPulsar1;
		sectorsPulsar1 = new Sector[permission.Length+1];
		for(int i = 0; i<permission.Length;i++){
			sectorsPulsar1[i] = permission[i];
		}
		sectorsPulsar1 [permission.Length] = sector;
	}

	void AddSectors2Pulsar(Sector sector){
		Sector[] permission = sectorsPulsar2;
		sectorsPulsar2 = new Sector[permission.Length+1];
		for(int i = 0; i<permission.Length;i++){
			sectorsPulsar2[i] = permission[i];
		}
		sectorsPulsar2 [permission.Length] = sector;
	}
	void AddNullSector(Sector sector){
		Sector[] permission = sectorsNull;
		sectorsNull = new Sector[permission.Length+1];
		for(int i = 0; i<permission.Length;i++){
			sectorsNull[i] = permission[i];
		}
		sectorsNull [permission.Length] = sector;
	}

	void AddBadSector(Sector sector){
		Sector[] permission = sectorsBad;
		sectorsBad = new Sector[permission.Length + 1];
		for(int i = 0;i<permission.Length;i++){
			sectorsBad[i] = permission[i];
		}
		sectorsBad [permission.Length] = sector;
	}

	void AddSector(){
		int colum;
		int line;
		int numGenerationSector;
		lock (locker) {
			colum = GenerateColum;
			line = GenerateLine;
			numGenerationSector = this.numGenerationSector;
			this.numGenerationSector++;
			NextSector ();
		}
		int minX = (colum - centerColumAndLine)*config.widhtSector;
		int minY = (line - centerColumAndLine)*config.heightSector;
		Sector sector = new Sector (minX, minY);
		sector.InitializeColumAndLine (colum,line);

		lock(lockerSector){
			if (numGenerationSector < numsSetors) {
				sectors [numGenerationSector] = sector;
//				Debug.Log (numGenerationSector);
				sector.DebugSector ();
				sectorsTable [line,colum] = sector;
			}
		}
		lock (Test.test.locer) {
			if (numGenerationSector >= numsSetors - 1) {
				coruntineCan = false;
				timeJudjeSector = true;
			//	
			}
		}
	}

	void AddSectorIternal(Sector sector){
		Sector[] permission = sectorsInternal;
		sectorsInternal = new Sector[permission.Length+1];
		for(int i =0; i<permission.Length;i++){
			sectorsInternal[i] = permission [i];
		}
		sectorsInternal [permission.Length] = sector;
	}

	void AddSectorAverage(Sector sector){
		Sector[] permission = sectorsAverag;
		sectorsAverag = new Sector[permission.Length+1];
		for(int i =0; i<permission.Length;i++){
			sectorsAverag[i] = permission [i];
		}
		sectorsAverag [permission.Length] = sector;
	}

	void AddSectorExternal(Sector sector){
		Sector[] permission = sectorsExternal;
		sectorsExternal = new Sector[permission.Length+1];
		for(int i =0; i<permission.Length;i++){
			sectorsExternal[i] = permission [i];
		}
		sectorsExternal [permission.Length] = sector;
	}

	void JudjeSector(){
		StartCoroutine ("JudjInternalSectors");
		StartCoroutine ("JudjAveragSectors");
		StartCoroutine ("JudjExternalSectors");
		timeJudjeSector = false;
	}

	IEnumerator JudjNeighborinSetors(){
		for(int i=0; i<sectors.Length; i++){
			if(sectors[i].colum>0){
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line,sectors[i].colum-1]);

			}
			if (sectors [i].line >0) {
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line-1,sectors[i].colum]);

			}
			if(sectors[i].colum<columSector-1){
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line,sectors[i].colum+1]);
						}
			if (sectors [i].line < lineSector-1) {
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line+1,sectors[i].colum]);
					}

			if(sectors[i].colum>0 && sectors[i].line>0){
					sectors [i].AddNeighborin (sectorsTable [sectors [i].line-1, sectors [i].colum-1]);
					
			}
			if (sectors [i].line < lineSector-1 && sectors[i].colum<columSector-1) {
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line+1,sectors[i].colum+1]);
			}
			if(sectors[i].colum>0 && sectors[i].line<lineSector-1){
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line+1,sectors[i].colum-1]);
			}
			if(sectors[i].colum<columSector-1&&sectors[i].line>0){
				sectors [i].AddNeighborin (sectorsTable[sectors[i].line-1,sectors[i].colum+1]);
//				Debug.Log ("Totu124124215ru");
			}
//			Debug.Log ("Frind" + sectors [i].sectorsNeighborin.Length);
		}
		yield return null;
	}

	IEnumerator GreateNullSectors(){
		int max = (config.persentNullAverageSectors+r.Next(4)) * numsSetors/100;
		int ret = 0 ;
		for(int i = 0;i<max;i++){
			int randomSector = r.Next (sectorsAverag.Length-1);
			if (sectorsAverag [randomSector].typeSector != 0 && !sectorsAverag [randomSector].EmtyNeighborin () && sectorsAverag [randomSector].full) {
				sectorsAverag [randomSector].full = false;

			} 

		}
			

		max = (config.persentNullExternalSectors +r.Next(3))* numsSetors/100;

		for(int i = 0;i<max;i++){
			int randomSector = r.Next (sectorsExternal.Length-1);
			if (sectorsExternal [randomSector].typeSector != 0 && !sectorsExternal [randomSector].EmtyNeighborin () && sectorsExternal [randomSector].full) {
				sectorsExternal [randomSector].full = false;
			} else {
			//	max++;
//				Debug.Log (randomSector);
			}

		}
		timeGreateConect = true;
		yield return null;
	}

	IEnumerator JudjInternalSectors(){
		int maxDistanseIternalSectors = config.widhtSector * config.persentInternalSectors * columSector/100/2;
		int minDistanseIternalSectors = 0;

		for(int i =0;i<sectors.Length; i ++){
			int distanseX = sectors [i].posX - centerX;
			int distanseY = sectors [i].posY - centerY;
			int distanseOfCenter = (int)Math.Sqrt ((distanseX*distanseX)+(distanseY*distanseY));
			if(distanseOfCenter<maxDistanseIternalSectors&&distanseOfCenter>minDistanseIternalSectors){
				sectors [i].typeSector = 1;
				sectors [i].full = true;
				AddSectorIternal( sectors [i]);
			}
		}
		avaibleIternal = true;
		yield return null;

	}

	IEnumerator JudjAveragSectors(){
		int minDistanseAverageSectors = config.widhtSector * config.persentInternalSectors * columSector/100/2 + config.widhtSector;
		int maxDistanseAverageSectors = config.widhtSector * config.persentAveragSectors * columSector/100/2 + minDistanseAverageSectors - config.widhtSector;


		for(int i =0;i<sectors.Length; i ++){
			int distanseX = sectors [i].posX - centerX;
			int distanseY = sectors [i].posY - centerY;
			int distanseOfCenter = (int)Math.Sqrt ((distanseX*distanseX)+(distanseY*distanseY));
			if(distanseOfCenter<maxDistanseAverageSectors&&distanseOfCenter>minDistanseAverageSectors){
				sectors [i].typeSector = 2;
				sectors [i].full = true;
				AddSectorAverage(sectors[i]);
			}
		}
		avaibleAverage = true;
		yield return null;

	}

	IEnumerator JudjExternalSectors(){
		int minDistanseExternalSectors = config.widhtSector * config.persentInternalSectors * columSector/100/2+
			config.widhtSector * config.persentAveragSectors * columSector/100/2 + config.widhtSector;
		int maxDistanseExternalSectors = config.widhtSector * config.persentExternalSectors * columSector/100/2 + minDistanseExternalSectors - config.widhtSector;


		for(int i =0;i<sectors.Length; i ++){
			int distanseX = sectors [i].posX - centerX;
			int distanseY = sectors [i].posY - centerY;
			int distanseOfCenter = (int)Math.Sqrt ((distanseX*distanseX)+(distanseY*distanseY));
			if(distanseOfCenter<maxDistanseExternalSectors&&distanseOfCenter>minDistanseExternalSectors){
				sectors [i].typeSector = 3;
				AddSectorExternal (sectors[i]);
				sectors [i].full = true;
			}
		}
		avaibleExternal = true;
		yield return null;

	}

	IEnumerator createPlanet(){
		int chanceAddPlanet;
		chanceAddPlanet = (config.maxPlanetGalaxy / numsSetors-sectors[0].GetNumsAllPlanet() / numsSetors);
	//	Debug.Log ("chanceAddPlanet"+ chanceAddPlanet);

		for (int i = 0; i < sectorsInternal.Length; i++) {
			int numsPlanetInSector = r.Next (config.minIternalPlanet+chanceAddPlanet,config.maxIternalPlanet+chanceAddPlanet);
			sectorsInternal [i].greated = true;
			for(int k = sectorsInternal[i].planets.Length ;k<numsPlanetInSector;k++){
				int x = r.Next (10);
				int y = r.Next (10);
				if (sectorsInternal [i].FreePointSector (x, y, config.minDistanceBetweenPlanets)&&sectorsInternal[i].full) {
						sectorsInternal [i].cretePlanets (x,y);
					chanceAddPlanet = (config.maxPlanetGalaxy / numsSetors-sectors[0].GetNumsAllPlanet() / numsSetors);
				//	Debug.Log ("chanceAddPlanet"+ chanceAddPlanet);
				}
			}
			sectorsInternal [i].greated = true;
		}
		for (int i = 0; i < sectorsAverag.Length; i++) {
			int numsPlanetInSector = r.Next (config.minAveragePlanet+chanceAddPlanet,config.maxAveragePlanet+chanceAddPlanet);
			sectorsAverag [i].greated = true;
			for(int k = sectorsAverag[i].planets.Length ;k<numsPlanetInSector;k++){
				int x = r.Next (10);
				int y = r.Next (10);
				if (sectorsAverag [i].FreePointSector (x, y, config.minDistanceBetweenPlanets)&& sectorsAverag[i].full) {
					sectorsAverag [i].cretePlanets (x,y);
					chanceAddPlanet = (config.maxPlanetGalaxy / numsSetors-sectors[0].GetNumsAllPlanet() / numsSetors);
//					Debug.Log ("chanceAddPlanet"+ chanceAddPlanet);
				}
			}
			sectorsAverag [i].greated = true;
		}
		for (int i = 0; i < sectorsExternal.Length; i++) {
			int numsPlanetInSector = r.Next (config.minExternalPlanet+chanceAddPlanet,config.maxExternalPlanet+chanceAddPlanet);
			sectorsExternal [i].greated = true;
			for(int k = sectorsExternal[i].planets.Length ;k<numsPlanetInSector;k++){
				int x = r.Next (10);
				int y = r.Next (10);
				if (sectorsExternal [i].FreePointSector (x, y, config.minDistanceBetweenPlanets)&& sectorsExternal[i].full) {
					sectorsExternal [i].cretePlanets (x,y);
					chanceAddPlanet = (config.maxPlanetGalaxy / numsSetors-sectors[0].GetNumsAllPlanet() / numsSetors);
			//		Debug.Log ("chanceAddPlanet"+ chanceAddPlanet);
				}
			}
			sectorsExternal [i].greated = true;
		}
		for(int i =0;i<sectors.Length;i++){
			if(sectors[i].planets.Length<1&&sectors[i].full){
				//sectors [i].bad = true;
				AddBadSector (sectors[i]);
			}
		}
		timeWorkBadSectors = true;
		sectors [0].DebugNumsPlanet ();
		yield return null;
	}

	IEnumerator WorkBadSectors(){
		for(int i =0;i<sectorsBad.Length;i++){
			GreateBadSectors (sectorsBad[i],0);
		}
		//Finish ();
		timeWorkPulsar = true;
		yield return null;
	}

	void GreateBadSectors(Sector sector, int attempt){
		if (attempt < 10) {
			int x = r.Next (10);
			int y = r.Next (10);
			if (sector.FreePointSector (x, y, config.minDistanceBetweenPlanets)) {
				sector.cretePlanets (x, y);
				GreateBadSectors (sector, attempt+1);
			} else {
				GreateBadSectors (sector, attempt+1);
			}
		}

	}

	IEnumerator CreateConnectSectors(){
		int numberNeigborin=0;
			for(int i=0;i<sectorsInternal.Length;i++){
			//sectorsInternal [i].full = false;
			numberNeigborin = sectorsInternal [i].ShowConnectNegborin(3);
			if(!sectorsInternal[i].full){
				continue;
			}
			if (numberNeigborin != -1) {
				if (!sectorsInternal [i].GreateConectNeghborin (r, numberNeigborin, config.minDistanceBetweenPlanets, config.maxLongDistance, 0)) {
					Debug.Log ("COLAPSE SECTOR CONECT");
				}
			} else {
				Debug.Log ("FATALL EROR");
			}

		}
		for(int i=0;i<sectorsInternal.Length;i++){
			//sectorsInternal [i].full = false;
			numberNeigborin = sectorsInternal [i].ShowConnectNegborin(4);
			if(!sectorsInternal[i].full){
				continue;
			}
			if (numberNeigborin != -1) {
				if (!sectorsInternal [i].GreateConectNeghborin (r, numberNeigborin, config.minDistanceBetweenPlanets, config.maxLongDistance, 0)) {
					Debug.Log ("COLAPSE SECTOR CONECT");
				}
			} else {
				Debug.Log ("FATALL EROR");
			}

		}
		for (int i = 0; i < sectorsAverag.Length; i++) {
			//sectorsInternal [i].full = false;
			if(!sectorsAverag[i].full){
				continue;
			}
			numberNeigborin = sectorsAverag [i].ShowConnectNegborin (3);

			if (numberNeigborin != -1) {
				if (!sectorsAverag [i].GreateConectNeghborin (r, numberNeigborin, config.minDistanceBetweenPlanets, config.maxLongDistance, 0)) {
					Debug.Log ("COLAPSE SECTOR CONECT");
				}
			} else {
				Debug.Log ("FATALL EROR");
			}
		}
		for (int i = 0; i < sectorsExternal.Length; i++) {
			//sectorsInternal [i].full = false;
			if(!sectorsExternal[i].full){
				continue;
			}
			numberNeigborin = sectorsExternal [i].ShowConnectNegborin (3);

			if (numberNeigborin != -1) {
				if (!sectorsExternal [i].GreateConectNeghborin (r, numberNeigborin, config.minDistanceBetweenPlanets, config.maxLongDistance, 0)) {
					Debug.Log ("COLAPSE SECTOR CONECT");
				}
			} else {
				Debug.Log ("FATALL EROR");
			}
		}
	
		for (int i = 0; i < sectorsAverag.Length; i++) {
			//sectorsInternal [i].full = false;
			if(!sectorsAverag[i].full){
				continue;
			}
			numberNeigborin = sectorsAverag [i].ShowConnectNegborin (4);

			if (numberNeigborin != -1) {
				if (!sectorsAverag [i].GreateConectNeghborin (r, numberNeigborin, config.minDistanceBetweenPlanets, config.maxLongDistance, 0)) {
					Debug.Log ("COLAPSE SECTOR CONECT");
				}
			} else {
				Debug.Log ("FATALL EROR");
			}
		}
		for (int i = 0; i < sectorsExternal.Length; i++) {
			//sectorsInternal [i].full = false;
			if(!sectorsExternal[i].full){
				continue;
			}
			numberNeigborin = sectorsExternal [i].ShowConnectNegborin (4);

			if (numberNeigborin != -1) {
				if (!sectorsExternal [i].GreateConectNeghborin (r, numberNeigborin, config.minDistanceBetweenPlanets, config.maxLongDistance, 0)) {
					Debug.Log ("COLAPSE SECTOR CONECT");
				}
			} else {
				Debug.Log ("FATALL EROR");
			}
		}
//		Debug.Log ("Planet Greated - " );
		for(int i = 0;i<sectorsInternal.Length;i++){
			sectorsInternal [i].DebugConect ();
		}
//		Debug.Log (sectorsInternal.Length);
		sectors [0].DebugNumsPlanet ();
		timeCreatePlanets = true;
		yield return null;
	}



	void Finish(){
		Test.test.sectors = sectors;
		Test.test.avake = true;

	}

	void Update () {
		if (generateSector) {
			if (columSector % 2 == 0) {
				centerColumAndLine = columSector / 2 + 1;
				centerX = -10;
				centerY = -10;
			} else {
				centerColumAndLine = (columSector - 1) / 2;
				centerX = 0;
				centerY = 0;
			}
			StartCoroutine ("Sector");
			StartCoroutine ("Sector");
			StartCoroutine ("Sector");
			StartCoroutine ("Sector");
			StartCoroutine ("Sector");
			generateSector = false;
		}
		if(timeJudjeSector){
			JudjeSector ();
		}
		if(avaibleAverage&&avaibleExternal&&avaibleIternal){
			avaibleExternal = false;
			avaibleAverage = false;
			avaibleIternal = false;
			timeNighborinJudje = true;
		}
		if(timeGreateNullSectors){
			timeGreateNullSectors = false;
			StartCoroutine ("GreateNullSectors");
		//	Finish ();
		}
		if(timeNighborinJudje){
			StartCoroutine ("JudjNeighborinSetors");
			timeNighborinJudje = false;
			timeGreateNullSectors = true;
			
		}

		if(timeCreatePlanets){
			timeCreatePlanets = false;
			StartCoroutine ("createPlanet");

		}

		if(timeGreateConect){
			timeGreateConect = false;
			StartCoroutine ("CreateConnectSectors");
		}
		if(timeWorkBadSectors){
			timeWorkBadSectors = false;
			StartCoroutine ("WorkBadSectors");
		}
		if(timeWorkPulsar){
			timeWorkPulsar = false;
			StartCoroutine ("WorkPulsar");
		}
	
	}
}
