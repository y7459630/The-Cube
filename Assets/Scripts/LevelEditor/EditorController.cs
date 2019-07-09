using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorController : MonoBehaviour{
    public GameObject Panel_Left;

    public Button Btn_OK;
    public Button Btn_Build;
    public Button Btn_Ruin;

    public Text Text_Message;
    public Text Text_BuildMode;

    public Image FadeImage;

    public int EditTool;
    //public bool MoveAndAct;

    public static string Status;

    void Awake(){
        Vector3 TempPos;
        EditTool = -1;
        Status = "SetPlayerPos";
        TempPos = Panel_Left.transform.position;
        Panel_Left.transform.position = new Vector3(-320, 540, 0);
        Text_Message.rectTransform.anchoredPosition += new Vector2(0, 100);
    }

    void FixedUpdate(){
        if(Status == "SetPlayerPos"){
            if(Text_Message.rectTransform.anchoredPosition.y >= 5 && FadeImage.color.a <= 0.5f){
                Text_Message.color += new Color(0, 0, 0, 0.1f);
                TextSlide(Text_Message, Vector2.zero, 0.1f);
                //Text_Message.rectTransform.anchoredPosition = Vector2.Lerp(Text_Message.rectTransform.anchoredPosition, Vector2.zero, 0.1f);
            }

            if(Global.Builder.GetComponent<BuildSetting>().CurrentFloor != null){
                Btn_OK.gameObject.SetActive(true);
            }
        }

        if(Status == "SetBuilding"){
            if(Panel_Left.transform.position.x <= 235){
                GameObjectSlide(Panel_Left, new Vector3(240, 540, 0), 0.1f);
            }
            //Panel_Left.transform.position = new Vector3(240, 540, 0);
        }
    }

    public void GameObjectSlide(GameObject Obj, Vector3 EndPos, float Speed){
        Obj.transform.position = Vector3.Lerp(Obj.transform.position, EndPos, Speed);
    }

    public void TextSlide(Text Obj, Vector2 EndPos, float Speed){
        Obj.rectTransform.anchoredPosition = Vector2.Lerp(Obj.rectTransform.anchoredPosition, EndPos, Speed);
    }

    public void ToEditMode(){
        GameObject[] EditButtons;
        GameObject Floor;

        Global.GameMode = "EditMode";
        EditButtons = GameObject.FindGameObjectsWithTag("EditFunction");
        foreach(GameObject Btn in EditButtons){
            Btn.GetComponent<Button>().interactable = true;
        }
        for(int i=0 ; i < Global.CurrentCube.GetComponent<CubeSetting>().AllFloors.Length ; i++){
            Floor = Global.CurrentCube.GetComponent<CubeSetting>().AllFloors[i];
            if(Floor.transform.childCount != 0){
                Floor.GetComponent<FloorInfo>().Building.transform.SetParent(Floor.transform);
            }
        }
    }

    public void ToPlayMode(){
        GameObject[] EditButtons;
        GameObject Floor;
        GameObject Cube;

        Global.GameMode = "PlayMode";
        Global.Builder.transform.position = Vector3.zero;
        EditButtons = GameObject.FindGameObjectsWithTag("EditFunction");
        foreach(GameObject Btn in EditButtons){
            Btn.GetComponent<Button>().interactable = false;
        }
        for(int i=0 ; i < Global.CurrentCube.GetComponent<CubeSetting>().AllFloors.Length ; i++){
            Floor = Global.CurrentCube.GetComponent<CubeSetting>().AllFloors[i];
            if(Floor.transform.childCount != 0){
                Cube = Floor.GetComponent<FloorInfo>().CurrentCube;
                Floor.GetComponent<FloorInfo>().Building.transform.SetParent(Cube.transform);
            }
        }
    }

    public void SwitchBuildMode(){
        EditTool = (EditTool + 1) % 2;

        switch(EditTool){
            case 0: Text_BuildMode.text = "按鈕式\n建造/刪除"; break;
            case 1: Text_BuildMode.text = "拖曳式\n建造/刪除"; break;
            //case 2: Text_BuildMode.text = "選取物件"; break;
        }

        //MoveAndAct = !MoveAndAct;
        //Text_BuildMode.text = MoveAndAct ? "拖曳式\n建造/刪除" : "按鈕式\n建造/刪除";

    }

    public void SetPlayerPos(){
        Global.Player.transform.position = Global.Builder.transform.position + Vector3.up;
        Global.Player.GetComponent<Rigidbody>().useGravity = true;
        Btn_OK.gameObject.SetActive(false);
        Text_Message.gameObject.SetActive(false);
        Global.Builder.transform.position = Vector3.zero;
        EditTool = 0;
        Status = "SetBuilding";
    }
}
