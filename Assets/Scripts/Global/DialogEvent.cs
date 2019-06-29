using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogEvent : MonoBehaviour{

    [System.Serializable]
    public struct DialogData{
        public int ID;
        public string Name;
        public string Content;
    }

    public Text Text_Name;
    public Text Text_Content;
    public List<DialogData> DialogList = new List<DialogData>();

    public void AddDialog(int _ID, string _Name, string _Content){
        DialogData Data;
        if(_Name != null && _Content != null){
            Data.ID = _ID;
            Data.Name = _Name;
            Data.Content = _Content;
            DialogList.Add(Data);
            Debug.Log("Add Data.");
        }else{
            Debug.Log("Null.");
        }

    }

    public void ShowDialog(int _ID){
        Debug.Log(DialogList[_ID]);
    }
}
