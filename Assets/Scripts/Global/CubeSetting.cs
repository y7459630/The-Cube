using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSetting : MonoBehaviour{

    public int ID;
    public GameObject BuildFloor;
    public GameObject CubeCore;
    public GameObject FloorHome;
    public GameObject[] AllFloors;
    public int CubeLayer;

    private int CubeBorder;
    private int Count = 0;

    void Awake(){
        if(ID == 1)
            Global.CurrentCube = gameObject;
        CubeBorder = CubeLayer * CubeLayer;
        AllFloors = new GameObject[CubeBorder*CubeBorder*6];
        CreateBuildFloor();
    }

    public void CreateBuildFloor(){
        
        GameObject Temp;

        for(int i = 0 ; i < 6 ; i++){
            for(int DirL = 0 ; DirL < CubeBorder ; DirL++){
                for(int DirR = 0 ; DirR < CubeBorder ; DirR++){
                    Temp = Instantiate(BuildFloor, new Vector3(BuildFloor.transform.position.x - DirL, BuildFloor.transform.position.y, BuildFloor.transform.position.z - DirR), Quaternion.identity);
                    AllFloors[Count] = Temp;
                    Count++;

                    if(i == 0){
                        Temp.name = "V3A" + DirL + DirR;
                        Temp.GetComponent<FloorInfo>().FloorID = 'A';
                        Temp.GetComponent<FloorInfo>().IsWalkable = true;
                        Temp.tag = "Walkable";
                    } else if(i == 1){
                        Temp.GetComponent<FloorInfo>().FloorID = 'B';
                        Temp.transform.RotateAround(CubeCore.transform.position, Vector3.forward, -90);
                        Temp.name = "V3B" + DirL + DirR;
                    } else if(i == 2){
                        Temp.GetComponent<FloorInfo>().FloorID = 'C';
                        Temp.transform.RotateAround(CubeCore.transform.position, Vector3.right, 90);
                        Temp.name = "V3C" + DirL + DirR;
                    } else if(i == 3){
                        Temp.GetComponent<FloorInfo>().FloorID = 'D';
                        Temp.transform.RotateAround(CubeCore.transform.position, Vector3.forward, 90);
                        Temp.name = "V3D" + DirL + DirR;
                    } else if(i == 4){
                        Temp.GetComponent<FloorInfo>().FloorID = 'E';
                        Temp.transform.RotateAround(CubeCore.transform.position, Vector3.right, -90);
                        Temp.name = "V3E" + DirL + DirR;
                    } else if(i == 5){
                        Temp.GetComponent<FloorInfo>().FloorID = 'F';
                        Temp.transform.RotateAround(CubeCore.transform.position, Vector3.right, 180);
                        Temp.name = "V3F" + DirL + DirR;
                    }

                    Temp.transform.SetParent(FloorHome.transform);
                }
            }
            
        }

        BuildFloor.SetActive(false);
        Invoke("ActiveBuilder", 1 * Time.deltaTime);
    }

    public void ActiveBuilder(){
        Global.Builder.SetActive(true);
    }

}
