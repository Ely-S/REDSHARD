#pragma strict
var doorObject:GameObject;
var openedDoor:boolean;
function Start () {
	openedDoor=true;
}

function Update () {
	
}

function OnTriggerEnter(other:Collider){
	if (openedDoor){
		doorObject.animation.Play();
		openedDoor=false;
		Debug.Log("open");
	}
}