using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Ето скрипт для проверки дистанции между космическими телами в Unity 3D 
//Его нужно навешывать на все тела которые планируется проверять на растояние в Unity 3D 
public class TestDistanse : MonoBehaviour {

    // Кординаты первого выбранного тела
    public static int posX = 0, posY = 0;

    //Кординаты второго выбранного тела
    public static int posX1 = 0, posY1 = 0;

    //При клике мыши в области колайдера космического тела 
    //происходит запоминания координат обекта 
    // еслы было выбрано оба тела выбрасывается в консоль дистанция в double
	void OnMouseDown(){
		if (posX == 0) {
			posX = (int)transform.position.x;
			posY = (int)transform.position.y;
		} else {
			posX1 = (int)transform.position.x;
			posY1 = (int)transform.position.y;
			double distanse = Math.Sqrt ( ((posX-posX1)*(posX-posX1))+((posY-posY1)*(posY-posY1)) );
			Debug.Log ("distanse -" +distanse);
			posX = 0;
			}
	
	}
}
