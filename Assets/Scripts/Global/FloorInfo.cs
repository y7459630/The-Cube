using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInfo : MonoBehaviour{
    [SerializeField]
    private bool HasBuilding;
    public GameObject Building;

	public char FloorID;
	public string OriginUnderCube;

	public bool IsWalkable;
    public bool Obstacle;
	
	private Ray Upray;
	private RaycastHit Upinfo;
	private RaycastHit Upinfo2;
	private Ray DownRay;
	private RaycastHit DownInfo;

	public GameObject UpFloor;
	public GameObject CurrentCube;

    void FixedUpdate(){
		if(IsWalkable){
			Upray = new Ray (gameObject.transform.position, Vector3.up);
			
			if (Physics.Raycast (Upray, out Upinfo, 2, 1 << 15) && Physics.Raycast (Upray, out Upinfo2, 5, 1 << 16)) {
				UpFloor = Upinfo2.collider.gameObject;
				//gameObject.GetComponent<Collider> ().enabled = false;
				Obstacle = true;
			} else if (Physics.Raycast (Upray, out Upinfo, 2, 1 << 15)) {
				UpFloor = null;
				//gameObject.GetComponent<Collider> ().enabled = true;
				Obstacle = true;
			} else if (Physics.Raycast (Upray, out Upinfo2, 5, 1 << 16)) {
				UpFloor = Upinfo2.collider.gameObject;
				//gameObject.GetComponent<Collider> ().enabled = false;
				Obstacle = false;
			} else {
				UpFloor = null;
				//gameObject.GetComponent<Collider> ().enabled = true;
				Obstacle = false;
			}
		}

		//DownRay = new Ray(transform.position, transform.up * (-1));
		DownRay.origin = transform.position + transform.up;
		DownRay.direction = transform.up * (-1);
		if(Physics.Raycast(DownRay, out DownInfo, 3, 1 << 9)){
			CurrentCube = DownInfo.transform.gameObject;
			if(OriginUnderCube.Equals(""))
				OriginUnderCube = DownInfo.transform.gameObject.name;
		}
	}

    public void SetBuilding(bool status, GameObject _Building){
        HasBuilding = status;
        Building = _Building;
    }

    public bool CheckBuilding(){
        return HasBuilding;
    }

    public GameObject GetBuilding(){
        return Building;
    }
}
