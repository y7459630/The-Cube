using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFormate : MonoBehaviour {

	public GameObject CubeLeader;
	public GameObject CubeHome;
	public GameObject[] RotatePlanes;
	public int FormationCount;

	void OnTriggerStay(Collider other){
		if (other.tag == "Cube") {
			other.transform.parent = CubeLeader.transform;
			if(CubeLeader.transform.childCount == FormationCount)
				gameObject.transform.position = new Vector3 (100, 100, 100);

		}

	}
}
