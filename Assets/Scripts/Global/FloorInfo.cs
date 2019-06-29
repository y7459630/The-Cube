using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInfo : MonoBehaviour{
    [SerializeField]
    private bool HasBuilding;
    [SerializeField]
    private GameObject Building;

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
