//#pragma strict
//public var
var speed:float;
var speedRun:float;
var speedRotate:float;
var gravity:float;
var jumpSpeed:float;
var jumpHeight:float;

//priv var

private var controller:CharacterController;
private var moveDirection:Vector3;
private var deltaTime:float;
private var characterContent;
private var runAnim:boolean;
private var walkAnim:boolean;


function Start () {


	controller = GetComponent("CharacterController");
	characterContent = transform.Find("Perso");
}

function Update () {

		//Pace
		deltaTime = Time.deltaTime;
		
		//On ne cours pas
		runAnim = false;
		walkAnim = false;

		if(controller.isGrounded){

		//Displacement
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			moveDirection = Vector3(0,0,Input.GetAxis("Vertical") * speedRun);
			runAnim = true;
		}else{
			moveDirection = Vector3(0,0,Input.GetAxis("Vertical") * speed);
			walkAnim = true;
			
		}
		
		//Gestion de l'animation
		if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)){
			if(!runAnim){
			
			//Si on ne cours pas
				characterContent.animation.CrossFade("Walk", 0.2);
			}else{
				characterContent.animation.CrossFade("Run", 0.2);
			}
			
		}else{
			characterContent.animation.CrossFade("idle", 0.2);
			}
		
		if(Input.GetKey(KeyCode.Space)){
			transform.position += transform.up * jumpSpeed  * 7 * Time.deltaTime;
			}
	
	}
	//Changer sur l'axe local
	moveDirection = transform.TransformDirection(moveDirection);
	//Rotation of Character
	transform.Rotate(Vector3(0,Input.GetAxis("Horizontal") * speedRotate * deltaTime,0));
	
	//Gravity
	moveDirection.y -= gravity * deltaTime;
	
	//Character controller displacement
	controller.Move(moveDirection * deltaTime);

		
}