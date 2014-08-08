using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class inputScript : MonoBehaviour {
	static List<GameObject> subscribers;
	public static float mouseXSensitivity = 2;
	public static float mouseYSensitivity = 0.05f;


	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		subscribers = new List<GameObject>();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public void Update () {


		if (Input.anyKey){
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){

				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("WKeyPressed",SendMessageOptions.DontRequireReceiver);
				}

			}

			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
				
				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("AKeyPressed",SendMessageOptions.DontRequireReceiver);
				}
				
			}

			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
				
				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("SKeyPressed",SendMessageOptions.DontRequireReceiver);
				}
				
			}

			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
				
				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("DKeyPressed",SendMessageOptions.DontRequireReceiver);
				}
				
			}

			if (Input.GetKey(KeyCode.P)){
				
				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("PKeyPressed",SendMessageOptions.DontRequireReceiver);
				}
				
			}

			if (Input.GetKey(KeyCode.Space)){
				
				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("SpaceKeyPressed",SendMessageOptions.DontRequireReceiver);
				}
				
			}

			if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)){
				
				for(int i = 0; i < subscribers.Count; i++){
					subscribers[i].SendMessage("ShiftKeyPressed",SendMessageOptions.DontRequireReceiver);
				}
				
			}
		}

		for(int i = 0; i < subscribers.Count; i++){

			subscribers[i].SendMessage("MouseMoved", new Vector2((Input.GetAxis("Mouse X")) * mouseXSensitivity,(Input.GetAxis("Mouse Y")) * mouseYSensitivity), SendMessageOptions.DontRequireReceiver);

		}

	}

	public static void SubscribeToImput(GameObject subscriber){

		subscribers.Add (subscriber);
	}
}
