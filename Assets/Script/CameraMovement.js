#pragma strict

private var targetCamera:Transform;
private var to:Vector3;
private var targetCharacter:Transform;

function Start () {
    targetCamera = GameObject.Find("Target").transform;
    targetCharacter = GameObject.Find("Samourai").transform;
}    

function Update () {
	transform.position = Vector3.Lerp(transform.position,targetCamera.position,0.1);
	
	to = targetCharacter.transform.position - transform.position;
	
	transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(to),0.1);

}