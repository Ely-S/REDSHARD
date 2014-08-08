#pragma strict

//Public Variables
var speed:float;
var speedRun:float;
var speedRotate:float;
var gravity:float;


//Private Variables

private var controller:CharacterController;
private var moveDirection:Vector3;
private var deltaTime:float;
private var characterContent;
private var runAnim:boolean;

function Start () {

	
	controller = GetComponent("CharacterController");
	characterContent = transform.Find("Personnage");

}

function Update () {

	//Cadence du temps
	deltaTime = Time.deltaTime;
	
	//On ne cours pas
	runAnim = false;
	
	//Deplacements Haut/Bas
	if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
		moveDirection = Vector3(0,0,Input.GetAxis("Vertical") * speedRun); 
		runAnim = true;
	}else{
		moveDirection = Vector3(0,0,Input.GetAxis("Vertical") * speed);	
	}
	
	
	/*if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)){
		if(!runAnim){
		characterContent.animation.CrossFade("walk", 0.2);
	}else{
		characterContent.animation.CrossFade("run", 0.2);
		
	}
	
	}else{
		characterContent.animation.CrossFade("idle", 0.2);
	}*/
		
	//Changer sur l'axe local
	moveDirection = transform.TransformDirection(moveDirection);
	
	//Rotation du personnage
	transform.Rotate(Vector3(0,Input.GetAxis("Horizontal") * speedRotate * deltaTime,0));
	
	//Gravité
	moveDirection.y -= gravity;
	
	//Deplacement du character controller
	controller.Move(moveDirection * deltaTime);
	
	 
}