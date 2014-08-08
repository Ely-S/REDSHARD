using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


	public bool listenToImput = true;

	public float distance;
	public Transform target;
	public Vector2 ofset;
	public float height;
	public float maxHeight;
	public float minHeight;

	bool thirdPerson = true;
	float originalDistance;
	public bool rotateAroundObject = false;

	float TimeOfPress = 0f;
	float DelayTime = 1f;

	// Use this for initialization
	void Start () {
		if (listenToImput) {
			inputScript.SubscribeToImput (transform.gameObject);
		}
		originalDistance = distance;
	}
	
	// Update is called once per frame
	void Update () {



		if(thirdPerson){
			Vector3 temp = target.position + new Vector3(ofset.x * (Mathf.Cos((target.eulerAngles.y) * Mathf.Deg2Rad)), ofset.y, ofset.x * (-(Mathf.Sin((target.eulerAngles.y) * Mathf.Deg2Rad))));

			transform.position = target.position + new Vector3(distance * (Mathf.Cos((target.eulerAngles.y + 90) * Mathf.Deg2Rad)), height, distance * (-(Mathf.Sin((target.eulerAngles.y + 90) * Mathf.Deg2Rad)))) + new Vector3(ofset.x * (Mathf.Cos((target.eulerAngles.y) * Mathf.Deg2Rad)),ofset.y, ofset.x * (-(Mathf.Sin((target.eulerAngles.y) * Mathf.Deg2Rad))));

			transform.LookAt(temp);
		}
		else{
			transform.position = target.position;
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);
		}

	}

	void PKeyPressed(){

		if(Time.realtimeSinceStartup > (TimeOfPress + DelayTime)) {
			TimeOfPress = Time.realtimeSinceStartup;

			thirdPerson = !thirdPerson;
		}
	}

	void ShiftKeyPressed(){


	}

	void MouseMoved(Vector2 mouseDeltaPos){
		if(thirdPerson){
			height = Mathf.Clamp(height - mouseDeltaPos.y, minHeight, maxHeight);
		}
		else{
			transform.transform.localEulerAngles += new Vector3(mouseDeltaPos.y * -30, 0, 0);
		}
	}
}
