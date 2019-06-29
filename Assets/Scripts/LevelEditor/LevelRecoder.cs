using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LevelRecoder : MonoBehaviour{

    LevelStatus SaveStatus = new LevelStatus();

    public void SaveLevel(){
        //LevelStatus Status = new LevelStatus();
        RecordBuilding();
        

        string SaveString = JsonUtility.ToJson(SaveStatus);
        StreamWriter File = new StreamWriter(Application.dataPath + "/Resources/LevelData.json");
        File.Write(SaveString);
        File.Close();

    }

    public void LoadLevel(){
        LevelStatus LoadStatus = LoadJsons.LoadJsonsFromFile();
        GameObject Temp;
        GameObject[] Floors;
        Floors = GameObject.FindGameObjectsWithTag("BuildFloor");

        // Clear Level
        for(int i = 0 ; i < Floors.Length ; i++){
            if(Floors[i].GetComponent<FloorInfo>().CheckBuilding() == true){
                for(int count = 0 ; count < Floors[i].transform.childCount ; count++){
                    DestroyImmediate(Floors[i].transform.GetChild(count).gameObject);
                }

                if(Floors[i].transform.childCount == 0){
                    Floors[i].GetComponent<FloorInfo>().SetBuilding(false, null);
                    
                }
            }
        }
        
        // Build Level
        for(int i = 0 ; i < LoadStatus.BuildDatas.Count ; i++){
            Temp = Instantiate(Resources.Load<GameObject>("Prefabs/" + LoadStatus.BuildDatas[i].BuildName));
            Temp.transform.position = LoadStatus.BuildDatas[i].BuildPosition;
            Temp.transform.rotation = LoadStatus.BuildDatas[i].BuildRotation;
            Temp.GetComponent<Renderer>().enabled = true;
            Temp.name = LoadStatus.BuildDatas[i].BuildName;
            Temp.transform.SetParent(GameObject.Find(LoadStatus.BuildDatas[i].FloorName).transform);
            GameObject.Find(LoadStatus.BuildDatas[i].FloorName).GetComponent<FloorInfo>().SetBuilding(true, Temp);
            
        }
        Global.Builder.GetComponent<BuildSetting>().CheckFloor();
        Debug.Log("Finish Loading !!");

    }

    public void RecordBuilding(){
        GameObject[] Floors;
        SaveStatus.BuildDatas.Clear();
        Floors = GameObject.FindGameObjectsWithTag("BuildFloor");
        for(int i = 0 ; i < Floors.Length ; i++){
            if(Floors[i].GetComponent<FloorInfo>().CheckBuilding() == true){
                BuildData Data = new BuildData();
                Data.ID = SaveStatus.BuildDatas.Count + 1;
                Data.BuildName = Floors[i].GetComponent<FloorInfo>().GetBuilding().name;
                Data.BuildPosition = Floors[i].GetComponent<FloorInfo>().GetBuilding().transform.position;
                Data.BuildRotation = Floors[i].GetComponent<FloorInfo>().GetBuilding().transform.rotation;
                Data.FloorName = Floors[i].name;
                SaveStatus.BuildDatas.Add(Data);
            }
        }
    }
}
