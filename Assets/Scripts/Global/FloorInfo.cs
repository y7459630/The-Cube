using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInfo : MonoBehaviour{
    [SerializeField]
    private bool HasBuilding;
    [SerializeField]
    private GameObject Building;

	public bool IsWalkable;
    public bool Obstacle;
	private Ray Upray;
	private RaycastHit Upinfo;
	private RaycastHit Upinfo2;
	public GameObject UpFloor;

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
