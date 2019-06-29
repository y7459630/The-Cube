using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildSetting : MonoBehaviour{

    public GameObject Building;
    public GameObject FloorScanner;
    public GameObject Builder_Skin;
    
    // 建造者所在地板
    [SerializeField]
    private GameObject CurrentFloor;

    // 建築物欲使用地板
    [SerializeField]
    private List<GameObject> BaseFloor = new List<GameObject>();

    private bool IsMoving;
    private bool FloorIsChecked;
    private bool MoveAndBuild = true;
    private int BuildType = 1; // Type 1 = 1*1*1

    private Ray ray;
    private Ray UpRay;
    private Ray DownRay;
    private RaycastHit BuildInfo;
    private RaycastHit UpInfo;
    private RaycastHit DownInfo;

    void Start(){
        CheckFloor();
    }

    void FixedUpdate(){
        if(FloorIsChecked == false){
            CheckFloor();

        }

        if(CheckBuildPermission() == true){
            Builder_Skin.GetComponent<Renderer>().material = Resources.Load("Materials/Global/Ghost") as Material;
        }else{
            Builder_Skin.GetComponent<Renderer>().material = Resources.Load("Materials/Global/RedGhost") as Material;
        }

        GrabMove();
    }

    void OnTriggerEnter(Collider other){
        if(other.transform.tag == "GhostWall"){
            transform.position = CurrentFloor.transform.position;
            transform.Translate(0, 0.5f, 0);
            CheckFloor();
        }
    }

    public void CheckFloor(){
        BaseFloor.Clear();

        // 檢查建造者所在的地板
        DownRay = new Ray(transform.position, -transform.up);
        if(Physics.Raycast(DownRay, out DownInfo, 1, 1 << 16)){
            CurrentFloor = DownInfo.transform.gameObject;
        }

        // 檢查建築物地基為種類1
        if(BuildType == 1){
            Builder_Skin.transform.localScale = new Vector3(1, 0.1f, 1);
            DownRay = new Ray(transform.position, -transform.up);
            if(Physics.Raycast(DownRay, out DownInfo, 1, 1 << 16) && DownInfo.transform.gameObject.GetComponent<FloorInfo>().CheckBuilding() == false){
                BaseFloor.Add(DownInfo.transform.gameObject);
            }
        }

        // 檢查建築物地基為種類3
        if(BuildType == 3){
            Builder_Skin.transform.localScale = new Vector3(3, 0.1f, 3);
            for(int DirL = 1 ; DirL >= -1 ; DirL--){
                for(int DirR = 1 ; DirR >= -1 ; DirR--){
                    FloorScanner.transform.position = transform.position;
                    FloorScanner.transform.Translate(DirR, 0, DirL);
                    DownRay.origin = FloorScanner.transform.position;
                    DownRay.direction = FloorScanner.transform.up * -1;
                    if(Physics.Raycast(DownRay, out DownInfo, 1, 1 << 16) && DownInfo.transform.gameObject.GetComponent<FloorInfo>().CheckBuilding() == false){
                        BaseFloor.Add(DownInfo.transform.gameObject);
                    }
                }
            }
        }

        FloorIsChecked = true;
        FloorScanner.transform.position = transform.position;
        
    }

    // 檢查建築許可
    public bool CheckBuildPermission(){
        if(BuildType == 1 && BaseFloor.Count == 1){
            return true;
        }

        if(BuildType == 3 && BaseFloor.Count == 9){
            return true;
        }

        return false;
    }

    // 拖曳式移動
    public void GrabMove(){
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButton(0) && Physics.Raycast(ray, out BuildInfo, 100, 1 << 16)){ // Layer16 = BuildFloor
            if(CurrentFloor != BuildInfo.transform.gameObject || IsMoving){
                IsMoving = true;
                transform.position = BuildInfo.transform.position;
                transform.rotation = BuildInfo.transform.rotation;
                transform.Translate(0, 0.5f, 0);
                CurrentFloor = BuildInfo.transform.gameObject;
                FloorIsChecked = false;
                

                IsMoving = false;
                //CurrentFloor = BuildInfo.transform.gameObject;
            }
            
            if(MoveAndBuild == true){
                if(Input.GetKey(KeyCode.LeftShift))
                    Invoke("Ruin", 0.05f);
                else
                    Invoke("Create", 0.05f);
            }
        }
    }

    // 按鈕式移動
    public void ButtonMove(string Dir){
        IsMoving = true;

        if(Dir == "Forward")
            transform.Translate(-1,  0,  0);
        if(Dir == "Back")
            transform.Translate( 1,  0,  0);
        if(Dir == "Left")
            transform.Translate( 0,  0, -1);
        if(Dir == "Right")
            transform.Translate( 0,  0,  1);

        IsMoving = false;
        FloorIsChecked = false;
    }

    // 生成建築物
    public void Create(){
        GameObject Temp;

        if(CheckBuildPermission() == true){
            if(CurrentFloor.GetComponent<FloorInfo>().CheckBuilding() == false){
                Temp = Instantiate(Building, transform.position, transform.rotation);
                Temp.GetComponent<Renderer>().enabled = true;
                Temp.transform.SetParent(CurrentFloor.transform);
                Temp.name = Building.name;
                CurrentFloor.GetComponent<FloorInfo>().SetBuilding(true, Temp);
            }
        }
    }

    // 摧毀建築物
    public void Ruin(){
        if(CurrentFloor.GetComponent<FloorInfo>().CheckBuilding() == true){
            // *** Temp Solution *** //
            for(int i = 0 ; i < CurrentFloor.transform.childCount ; i++){
                DestroyImmediate(CurrentFloor.transform.GetChild(i).gameObject);
            }

            if(CurrentFloor.transform.childCount == 0){
                CurrentFloor.GetComponent<FloorInfo>().SetBuilding(false, null);
                CheckFloor();
            }
        }
    }

    public void SwitchBuildMode(){
        Text BuildMode = GameObject.Find("Text_BuildMode").GetComponent<Text>();

        MoveAndBuild = !MoveAndBuild;
        BuildMode.text = MoveAndBuild ? "拖曳式建造" : "按鈕式建造";

    }

}
