using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//клас управления камерой в юнити 
public class Camer : MonoBehaviour {
    //Игровой обект камеры которой нужно управлять(Инициализируется в юнити)
    public GameObject camer;

    //bool Движится ли камера
	bool camerIsMove;

    //камера которой кправляем (Инициализируется в юнити)
    public Camera cam;

    //чуствительность мышы
	public float sensitivityMouse = 1000f;

    //Позиция камеры
	float Xpos, Ypos, zpos;

    //zoom камеры
    public float zoom = 10f;

    //Корунтина перемещения камеры
	IEnumerator MoveCamer(){
		while(true){
			if(Input.GetKey(KeyCode.LeftArrow)){
				Vector3 vector = new Vector3 (-10,0,0) + camer.transform.position;
				camer.transform.position = Vector3.MoveTowards (camer.transform.position,vector,10f);
			}
			if(Input.GetKey(KeyCode.RightArrow)){
				Vector3 vector = new Vector3 (10,0,0) + camer.transform.position;
				camer.transform.position = Vector3.MoveTowards (camer.transform.position,vector,10f);
			}
			if(Input.GetKey(KeyCode.UpArrow)){
				Vector3 vector = new Vector3 (0,10,0) + camer.transform.position;
				camer.transform.position = Vector3.MoveTowards (camer.transform.position,vector,10f);
			}
			if(Input.GetKey(KeyCode.DownArrow)){
				Vector3 vector = new Vector3 (0,-10,0) + camer.transform.position;
				camer.transform.position = Vector3.MoveTowards (camer.transform.position,vector,10f);
			}

			yield return null;
		}
		yield return null;
	}

    //Запуск корунтины перемещения камеры
	void Start () {
		StartCoroutine ("MoveCamer");
		zpos = cam.transform.position.z;
	}
	
    //Управление зумом камеры
	void Update () {
		float zom =zoom - Input.GetAxis ("Mouse ScrollWheel")*5f;
		if (zom > 5f & zom < 50f) {
			zoom = zom;
			cam.orthographicSize = zoom;
		}

	}
}
