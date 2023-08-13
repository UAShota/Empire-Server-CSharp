using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Sector  {
	public int numSector;
	public int posX;
	public int posY;
	public int colum;
	public int line;
	//varible 1=secto Iternal 2= sectorAverage 3=sectorExternal
	public int typeSector=0;
	public int[] numsConnectedSectors = {-1,-1,-1,-1,-1,-1};

	int minX;
	int minY;
	public bool full;
	public CosmicBody[] planets = new CosmicBody[0];
	public Sector[] sectorsNeighborin = new Sector[0];
	public bool greated= false;
	public byte numsConnectneighborin = 0;
	public static int numsAllPlanet = 0;
	public bool bad = false;



	public Sector(int minX, int minY){
		this.minX = minX;
		this.minY = minY;
		this.posX = minX + 5;
		this.posY = minY + 5;
	}

	public void DebugSector(){
//		Debug.Log ("posX -"+posX+" posY -"+posY+" minX - "+minX+" maxX -"+maxX+" minY -"+minY+" maxY -"+ maxY);
	}

	public void cretePlanets(int posX, int posY){
		CosmicBody[] permission = planets;
		planets = new CosmicBody[planets.Length+1];
		for(int i = 0;i<permission.Length;i++){
			planets [i] = permission [i];
		}
		planets [permission.Length] = new CosmicBody (posX+minX,posY+ minY);
		numsAllPlanet++;
		}

	public void cretePlanets(int posX, int posY,int type){
		CosmicBody[] permission = planets;
		planets = new CosmicBody[planets.Length+1];
		for(int i = 0;i<permission.Length;i++){
			planets [i] = permission [i];
		}
		planets [permission.Length] = new CosmicBody (posX+minX,posY+ minY,type);
		numsAllPlanet++;
	}

	public void AddNeighborin(Sector sector){
		Sector[] permission = sectorsNeighborin;
		sectorsNeighborin = new Sector[sectorsNeighborin.Length+1] ;
		for(int i =0; i<permission.Length;i++){
			sectorsNeighborin [i] = permission [i];
		}
		sectorsNeighborin [permission.Length] = sector;

	}

	public void InitializeColumAndLine(int colum, int line){
		this.colum = colum;
		this.line = line;
	}

	public bool EmtyNeighborin (){
		bool res = false;
		for(int i = 0;i<sectorsNeighborin.Length;i++){
			
			if (sectorsNeighborin [i].typeSector != 0 && sectorsNeighborin [i].full == false) {
				res = true;
			}
//			Debug.Log (res);
		}
		return res;
	}

	public bool FreePointSector(int x,int y,int minDistance){
		bool result = true;
		x = x + minX;
		y = y + minY;
		if(!FreePoint(x,y,minDistance)){
			result = false;
			return result;
		}
		for(int i = 0; i<sectorsNeighborin.Length;i++){
			if(sectorsNeighborin[i].full){
				if (sectorsNeighborin [i].typeSector != typeSector &&
					!sectorsNeighborin [i].FreePoint (x, y, 12)) {
						return false;
					}

				if (!sectorsNeighborin [i].FreePoint (x, y, minDistance)) {
					result = false;
					return result;
				}
			}
		}
		return result;
	}

	public void FreePointConectPulsar(){
		double distance1=20;
		double distance2=20;

		int num1Friend=0;
		int num2Friend=0;

		int typeNeigborin1=0;
		int typeNeigborin2=0;

		cretePlanets (5, 5 ,0);

		for(int i=0;i<sectorsNeighborin.Length;i++){
			if (sectorsNeighborin[i].typeSector != 0&&
				sectorsNeighborin[i].full) {
				double	distance = Math.Sqrt (((sectorsNeighborin [i].posX - posX) * (sectorsNeighborin [i].posX - posX)) +
				((sectorsNeighborin [i].posY - posY) * (sectorsNeighborin [i].posY - posY)));
				if(distance<distance1){
					num1Friend = i;
					distance1 = distance;
					typeNeigborin1 = sectorsNeighborin [i].typeSector;
				}
			}
		}
		for(int i=0;i<sectorsNeighborin.Length;i++){
			if (sectorsNeighborin[i].typeSector != 0&&
				sectorsNeighborin[i].full
				&&sectorsNeighborin[i].typeSector !=typeNeigborin1) {
				double	distance = Math.Sqrt (((sectorsNeighborin [i].posX - posX) * (sectorsNeighborin [i].posX - posX)) +
					((sectorsNeighborin [i].posY - posY) * (sectorsNeighborin [i].posY - posY)));
				if(distance<distance2){
					num2Friend = i;
					distance2 = distance;
					typeNeigborin2 = sectorsNeighborin [i].typeSector;
				}
			}
		}
		System.Random r = new System.Random ();
		GreateConectNeghborin (r,num1Friend,5,12,0);
		GreateConectNeghborin (r,num2Friend,5,12,0);

	}

	public bool FreePoint(int posX, int posY,int minDistance){
		bool result = true;
		double distance;
		for(int i = 0;i<planets.Length;i++){
			distance = Math.Sqrt((planets [i].posX-posX)*(planets [i].posX-posX) + (planets [i].posY-posY)*(planets [i].posY-posY));
			if(distance<minDistance){
				result = false;
				return result;
			}
		}
		return result;
	}

	bool ValueOfConect(int numFr){
		for(byte i = 0; i<numsConnectneighborin;i++){
			if (numsConnectedSectors [i] == numFr) {
				return true;
			}
		}
		return false;
	}

	public int ShowConnectNegborin(byte numsConect ){
		int result=-1;
		for(int i = 0; i<sectorsNeighborin.Length;i++){
			if (sectorsNeighborin [i].typeSector == typeSector){
				
				if (sectorsNeighborin [i].full) {
					if (sectorsNeighborin [i].numsConnectneighborin < numsConect &&
					  !ValueOfConect (i)) {
						result = i;			
						return result;
					}
				}
			}
		}
		return result;

	}
	public void DebugConect (){
//		Debug.Log ("Conect to" + numsConnectedSectors [0] + numsConnectedSectors [1] + numsConnectedSectors [2]);
	}
	void AceptConnect(Sector sector){
		for (int i = 0; i<sectorsNeighborin.Length;i++){
			if (sectorsNeighborin [i] == sector) {
				numsConnectedSectors[numsConnectneighborin] = i;
//				numsConnectneighborin ++;
				//Debug.Log ("ConI!II!I!");

			}
		}
	}

	void ConectOk(int numsNeighborin){
		Debug.Log (numsConnectedSectors.Length);
		Debug.Log (numsConnectneighborin);
		numsConnectedSectors [numsConnectneighborin] = numsNeighborin;
		numsConnectneighborin ++;
		sectorsNeighborin [numsNeighborin].AceptConnect (this);
	}

	public bool GreateConectNeghborin(System.Random r,int numsNeighborin,int minDistance,int maxDistance, int attempt){
		int myX;
		int myY;
		int frX;
		int frY;
		int x;
		int y;
		int x1;
		int y1;
		int numPlan;
		int numFriendPlan;
		double distance;
		if(attempt>100){
			bad = false;
			return false;

		}
		if (planets.Length > 0 && attempt<30) {
			if (sectorsNeighborin [numsNeighborin].planets.Length > 0 && attempt<1) {
				if (AvaibleFriendConnectPointes (planets, sectorsNeighborin [numsNeighborin].planets, minDistance, maxDistance)) {
					ConectOk (numsNeighborin);
					return true;
				}else{
					GreateConectNeghborin (r,numsNeighborin,minDistance,maxDistance,attempt+1);
				}

			} else {

				frX = r.Next (10);
				frY = r.Next (10);
				if (AvaibleFriendConnectPointes (planets, frX, frY, minDistance, maxDistance)&&
					sectorsNeighborin [numsNeighborin].FreePointSector(frX,frY,minDistance)) {
					sectorsNeighborin [numsNeighborin].cretePlanets (frX,frY);
					ConectOk (numsNeighborin);
//					Debug.Log ("создали в соседа планету для конекта");
					return true;
				} else {
					GreateConectNeghborin (r,numsNeighborin,minDistance,maxDistance,attempt+1);
				}
				
			}
		} else {
			if (sectorsNeighborin [numsNeighborin].planets.Length > 0 && attempt<60) {
				myX = r.Next (10);
				myY = r.Next (10);

				if(AvaibleFriendConnectPointes(myX,myY,sectorsNeighborin[numsNeighborin].planets,minDistance,maxDistance)&&
					FreePointSector(myX,myY,minDistance)){
					ConectOk (numsNeighborin);
					cretePlanets (myX,myY);
//					Debug.Log ("создали у себяя планету для конекта");
					return true;
				}	else {
					GreateConectNeghborin (r,numsNeighborin,minDistance,maxDistance,attempt+1);
				}


			} else {
				myX = r.Next (10);
				myY = r.Next (10);
	
				frX = r.Next (10);
				frY = r.Next (10);

				if(AvaibleFriendConnectPoints(myX,myY,frX,frY,numsNeighborin,minDistance,maxDistance)
					&&FreePointSector(myX,myY,minDistance)){
					cretePlanets (myX,myY);
					if (AvaibleFriendConnectPointes (planets, frX, frY, minDistance, maxDistance)&&
						sectorsNeighborin [numsNeighborin].FreePointSector (frX, frY,minDistance)) {
							sectorsNeighborin [numsNeighborin].cretePlanets (frX, frY);
							ConectOk (numsNeighborin);
//							Debug.Log ("создали в соседа планету для конекта");
							return true;
						 
					}else {
						GreateConectNeghborin (r,numsNeighborin,minDistance,maxDistance,attempt+1);
					} 
				}	else {
					GreateConectNeghborin (r,numsNeighborin,minDistance,maxDistance,attempt+1);
				}
			}
		}
		return true;

	}
	bool AvaibleFriendConnectPointes(CosmicBody[] planets, int frX,int frY,int minDistance,int maxDistance){
		double distanse;
		bool result = false;

		for (int i =0; i<planets.Length;i++){
			distanse = Math.Sqrt ((frX-planets[i].posX)*(frX-planets[i].posX)+(frY-planets[i].posY)*(frY-planets[i].posY));
			if (distanse > minDistance) {
				if (distanse < maxDistance) {
					result = true;
				} 
			} else {
				return false;
			}
		}
		return result;

	}

	bool AvaibleFriendConnectPointes(int myX,int myY, CosmicBody[] planets,int minDistance,int maxDistance){
		double distanse;
		bool result = false;

		for (int i =0; i<planets.Length;i++){
			distanse = Math.Sqrt ((myX-planets[i].posX)*(myX-planets[i].posX)+(myY-planets[i].posY)*(myY-planets[i].posY));
			if (distanse > minDistance) {
				if (distanse < maxDistance) {
					result = true;
				} 
			} else {
				return false;
			}
		}
		return result;
	}

	bool AvaibleFriendConnectPointes(CosmicBody[] planets, CosmicBody[] planets1,int minDistance,int maxDistance){
		bool result = false;
		double distance;
		for (int i =0; i<planets.Length;i++){
			for(int k=0;k<planets1.Length;k++){
				distance = Math.Sqrt ((planets[i].posX - planets1[k].posX)*(planets[i].posX - planets1[k].posX)+
					(planets[i].posY - planets1[k].posY)*(planets[i].posY - planets1[k].posY));
				if (distance > minDistance) {
					if (distance < maxDistance) {
						result = true;
					}
				} else {
					return false;
				}
			}

	}
		return result;
	}


	bool AvaibleFriendConnectPoints(int myX,int myY,int frX,int frY,int numFr,int minDistance,int maxDistance){
		if(!FreePointSector(myX,myY,minDistance)){
			return false;
		}
		if(!sectorsNeighborin[numFr].FreePointSector(frX,frY,minDistance)){
			return false;
		}
		myX = myX + minX;
		myY = myY + minY;
		frX = frX + sectorsNeighborin [numFr].minX;
		frY = frY + sectorsNeighborin [numFr].minY;

		double distance = Math.Sqrt (((myX-frX)*(myX-frX)+ (myY - frY)*(myY - frY)));
		if (distance < minDistance) {
			return false;
		}
		if(distance>maxDistance){
			return false;
		}
		return true;

	}

	public void DebugNumsPlanet(){
		Debug.Log ("numsAllPlanet- "+numsAllPlanet);
	}

	public int GetNumsAllPlanet(){
		return numsAllPlanet;
	}

	public bool FriendIternal(){
		for(int i=0;i<sectorsNeighborin.Length;i++){
			if(sectorsNeighborin[i].typeSector==1){
				return true;
			}
		}
		return false;
	}

	public bool FriendAverag(){
		for(int i=0;i<sectorsNeighborin.Length;i++){
			if(sectorsNeighborin[i].typeSector==2){
				return true;
			}
		}
		return false;
	}
	public bool FriendExternal(){
		for(int i=0;i<sectorsNeighborin.Length;i++){
			if(sectorsNeighborin[i].typeSector==3){
				return true;
			}
		}
		return false;
	}

}
