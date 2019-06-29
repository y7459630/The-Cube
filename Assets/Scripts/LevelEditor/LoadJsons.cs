using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadJsons : MonoBehaviour{
    public static LevelStatus LoadJsonsFromFile(){
        BinaryFormatter BF = new BinaryFormatter();
        if(!File.Exists(Application.dataPath + "/Resources/LevelData.json")){
            return null;
        }

        StreamReader SR = new StreamReader(Application.dataPath + "/Resources/LevelData.json");
        if(SR == null){
            return null;
        }

        string Json = SR.ReadToEnd();
        SR.Close();
        if(Json.Length > 0){
            return JsonUtility.FromJson<LevelStatus>(Json);
        }

        return null;
    }
}
