using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class LevelStatus{
    public List<BuildData> BuildDatas = new List<BuildData>();
    
}

[Serializable]
public class BuildData{
    public int ID;
    public string BuildName;
    public Vector3 BuildPosition;
    public Quaternion BuildRotation;
    public string FloorName;
}
