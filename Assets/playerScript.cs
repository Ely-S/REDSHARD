using UnityEngine;
using System.Collections;

public class playerScript : MonoBehaviour {

	public float playerSpeed = 0f;
	public float rotateSpeed = 0f;

	// Use this for initialization
	void Start () {
	 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.UpArrow)){//

			rigidbody.AddRelativeForce(Vector3.forward * playerSpeed);
		}
		if (Input.GetKey(KeyCode.DownArrow)){

			rigidbody.AddRelativeForce(Vector3.forward * -playerSpeed);
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
			rigidbody.AddRelativeTorque (Vector3.up * -rotateSpeed);

		}
		if (Input.GetKey(KeyCode.RightArrow)){
			rigidbody.AddRelativeTorque (Vector3.up * rotateSpeed);

		}
		
	

	}
}
