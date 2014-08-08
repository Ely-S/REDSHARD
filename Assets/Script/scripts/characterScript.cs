using UnityEngine;
using System.Collections;

public class characterScript : MonoBehaviour {

	public bool listenToImput = true;
	public float acceleration;
	public float jumpForce;
	public float maxWalkSpeed;
	public float maxRunSpeed;
	bool canRun = false;
	public new LayerMask whatIsntGround;
	// Use this for initialization
	void Start () {
		if (listenToImput) {
			inputScript.SubscribeToImput (transform.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(Mathf.Abs(rigidbody.velocity.x) > maxWalkSpeed && !canRun){
			rigidbody.velocity = new Vector3(maxWalkSpeed * Mathf.Sign(rigidbody.velocity.x), rigidbody.velocity.y, rigidbody.velocity.z);
		}

		if(Mathf.Abs(rigidbody.velocity.z) > maxWalkSpeed && !canRun){
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, maxWalkSpeed * Mathf.Sign(rigidbody.velocity.z));
		}

		if(Mathf.Abs(rigidbody.velocity.x) > maxRunSpeed && canRun){
			rigidbody.velocity = new Vector3(maxRunSpeed * Mathf.Sign(rigidbody.velocity.x), rigidbody.velocity.y, rigidbody.velocity.z);
		}
		
		if(Mathf.Abs(rigidbody.velocity.z) > maxRunSpeed && canRun){
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, maxRunSpeed * Mathf.Sign(rigidbody.velocity.z));
		}
		canRun = false;
	}

	void MouseMoved(Vector2 mouseDeltaPos){
		transform.eulerAngles = transform.eulerAngles + new Vector3(0, mouseDeltaPos.x, 0);
	}

	void WKeyPressed(){

		rigidbody.AddRelativeForce(Vector3.forward * acceleration);
	}

	void AKeyPressed(){
		rigidbody.AddRelativeForce(Vector3.left * acceleration);
	}

	void SKeyPressed(){
		rigidbody.AddRelativeForce(Vector3.back * acceleration);
	}

	void DKeyPressed(){
		rigidbody.AddRelativeForce(Vector3.right * acceleration);
	}

	void ShiftKeyPressed(){
		canRun = true;
	}

	void SpaceKeyPressed(){
		if(Physics.Linecast(transform.position, transform.position + new Vector3(0,-2, 0), whatIsntGround)){
			rigidbody.AddRelativeForce(Vector3.up * jumpForce);
		}
	}


}
