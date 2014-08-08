#pragma strict

var baseComponents:GameObject[];  //base components are the larger, less moving parts of the infestation
var baseComponentsNumber:int=2; //how much of them has to be generated
var details:GameObject[];  //details are the more noticeable, more moving parts - tentacles, particle systems, etc.. The orientation of the details matters, we dont want them to clip into the body
var detailsNumber:int=4;

var drawGizmos:boolean=true;	

//var grow:boolean=false; //the infestation appers instantly, or grows?

var xSize:float=0.5;  //this should be defined by the user in the inspector. This is the gizmo where the infestation gets generated
var ySize:float=0.1;	
var zSize:float=0.1;

var detailXDeviation:float=30;
var detailYDeviation:float=30;
var detailZDeviation:float=30;

private var i:int;  //used for "for" loops
private var justCreated:GameObject; //this is always the just created component, when we need to rotate it or anything


function Start () {
if (baseComponentsNumber > 0 && baseComponents.length > 0) 
	{
	for (i = 0; i < baseComponentsNumber; i++)  //we generate the baseComponents, then scatter them along the box defined by the user, and randomly rotate them
		{
		justCreated=Instantiate(baseComponents[Mathf.Floor(Random.Range(0, baseComponents.length))], transform.position, transform.rotation);	
		justCreated.transform.parent=transform;

		justCreated.transform.localPosition.x+=Random.Range(-xSize/2, xSize/2);  //setting its random position among these boundaries
		justCreated.transform.localPosition.y+=Random.Range(-ySize/2, ySize/2);
		justCreated.transform.localPosition.z+=Random.Range(-zSize/2, zSize/2);
		
		justCreated.transform.rotation=Random.rotation;
		}
	}

if (detailsNumber > 0 && details.length > 0) 
	{
	for (i = 0; i < detailsNumber; i++)  //we generate the details, then scatter them along the box defined by the user
		{
		justCreated=Instantiate(details[Mathf.Floor(Random.Range(0, details.length))], transform.position, transform.rotation);	
		justCreated.transform.parent=transform;
		
		justCreated.transform.localPosition.x+=Random.Range(-xSize/2, xSize/2);  //setting its random position among these boundaries
		justCreated.transform.localPosition.y+=Random.Range(-ySize/2, ySize/2);
		justCreated.transform.localPosition.z+=Random.Range(-zSize/2, zSize/2);
				
		//details aren't rotated totally randomly, just in the boundaries of the defined deviations
		justCreated.transform.Rotate(Vector3(Random.Range(-detailXDeviation, detailXDeviation), Random.Range(-detailYDeviation, detailYDeviation), Random.Range(-detailZDeviation, detailZDeviation)));
		
		}
	}
}



function OnDrawGizmos () {  //this draws a gizmo in the editor, to show where the infestation will happen if the script is activated

	if (drawGizmos==true)
		{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color (1,0,0,.4);
		Gizmos.DrawCube (Vector3.zero, Vector3 (xSize,ySize,zSize)); //The gizmo where the components are actually generated
		
		Gizmos.DrawCube (Vector3(0, 0.25, 0), Vector3 (0.1,0.5,0.1));  //this shows the general direction where the details will point
		
		Gizmos.color = Color (1,0,0,.15);
		Gizmos.DrawCube (Vector3.zero, Vector3 (xSize+0.2,ySize+0.2,zSize+0.2));  //a larger, fainter gizmo help placement even if the main gizmo is fully under a mesh
		}
}