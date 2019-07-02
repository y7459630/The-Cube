using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {



	// Player的移動機制
	// MoveL~MoveD為Player相對於目的地的距離
	float MoveL;
	float MoveR;
	float MoveD;
	//float RotateSpeed = 0.25f;
	Quaternion RotateDir;
	Vector3 FixedHeight;
	Vector3 MoveToTarget;
	public static float MoveSpeed = 4f;

	// 限制行動方向
	bool LockDirR = false;
	bool LockDirL = false;
	public bool LockRotation;

	// 偵測主角所在地板
	Ray DownRay;
	RaycastHit hitinfo;
	public static GameObject CurrentFloor;
	public static string MoveMode;

	float BoxPosY;
	//private InsideMode inside;
	//public static bool isInside;

	private float Stime;
	private bool isOut;
	private GameObject TheEntry;

	private Vector3 OldPlayerPos;
	private Quaternion OldPlayerRot;
	private Vector3 OldCamPos;
	private float OldCamView;
	private bool DoOnce;
	
	void Awake(){
		
	}

	void Start () {
		PlayerSetting();
		FixedHeight = new Vector3 (0, 1, 0);
		MoveMode = "SmartWalk";
	}



	/*public void EditorSetting(GameObject _ScriptObj, int _PlayMode){
		if (_PlayMode == 0) {
			GameObject.Find ("GlobalScripts").GetComponent<PathController> ().enabled = false;
			GameObject[] Obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
			foreach (GameObject obj in Obstacles)
				obj.GetComponent<Collider> ().enabled = false;
		} else if (_PlayMode == 1) {
			GameObject.Find ("GlobalScripts").GetComponent<PathController> ().enabled = true;
			GameObject[] Obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
			foreach (GameObject obj in Obstacles)
				obj.GetComponent<Collider> ().enabled = true;
			_ScriptObj.GetComponent<ChangeMode>().FinishPlayerSetting = true;
		}
	}*/

	void Update(){
		if(Global.Player != null)
			DownRay = new Ray (Global.Player.transform.position, Vector3.down);
		if (Physics.Raycast (DownRay, out hitinfo, 5, 1 << 16))
			CurrentFloor = hitinfo.collider.gameObject;
	}

	void FixedUpdate () {
		// 計算至目標的距離
		if (Global.BeTouchedObj != null) {
			MoveToTarget = Global.BeTouchedObj.transform.position;
			MoveL = -(MoveToTarget.z - Global.Player.transform.position.z);
			MoveR = -(MoveToTarget.x - Global.Player.transform.position.x);
			MoveD = -(MoveToTarget.y - Global.Player.transform.position.y);
		}


		if (Global.IsRotating || Global.IsPreRotating)
			LockRotation = true;

		if (!Global.IsPushing) {
			LockDirR = LockDirL = false;
		}

		// 滑鼠右鍵：取消推箱子模式
		if(Input.GetMouseButtonUp(1) && Global.IsPushing && Global.PlayerMove == false && Global.BePushedObj != null){
			LockDirR = LockDirL = false;
			MoveSpeed = 4;

			//Global.BePushedObj.GetComponent<Renderer> ().material = Resources.Load ("Materials/Global/White")as Material;
			Global.BePushedObj.transform.parent = GameObject.Find ("MoveableGroup").transform;
			Global.BePushedObj.transform.position = new Vector3(Global.BePushedObj.transform.position.x, BoxPosY, Global.BePushedObj.transform.position.z);
			Global.BePushedObj = null;
			CancelMoving (new Vector3(CurrentFloor.transform.position.x, transform.position.y, CurrentFloor.transform.position.z));
			Global.IsPushing = false;
		}


		// 主角移動機制
		if (Global.PlayerMove && !Global.IsRotating && !Global.IsPreRotating && Global.IsPushing) {
			LockRotation = false;
			if (Mathf.Abs (MoveR) > 0.05f && LockDirR == false) {
				if (MoveR > 0) {
					Global.Player.transform.position += new Vector3 (-MoveSpeed * Time.deltaTime, 0, 0);
					if(!Global.IsPushing)
						RotateDir = Quaternion.Euler (0, -90, 0);

				} else if (MoveR < 0) {
					Global.Player.transform.position += new Vector3 (MoveSpeed * Time.deltaTime, 0, 0);
					if(!Global.IsPushing)
						RotateDir = Quaternion.Euler (0, 90, 0);

				}
			} else if (Mathf.Abs (MoveL) > 0.05f && LockDirL == false) {
				if (MoveL > 0) {
					Global.Player.transform.position += new Vector3 (0, 0, -MoveSpeed * Time.deltaTime);
					if(!Global.IsPushing)
						RotateDir = Quaternion.Euler (0, 180, 0);

				} else if (MoveL < 0) {
					Global.Player.transform.position += new Vector3 (0, 0, MoveSpeed * Time.deltaTime);
					if(!Global.IsPushing)
						RotateDir = Quaternion.Euler (0, 0, 0);

				}
			} else if(Mathf.Abs (MoveD) < 3f){
				if (Global.IsPushing) {
					CancelMoving (new Vector3(CurrentFloor.transform.position.x, transform.position.y, CurrentFloor.transform.position.z));
				} else {
					LockDirR = LockDirL = false;
					PlayerStop ();
					print("Cancel");


				}
			}
		}



	}

	void PlayerSetting(){
		if(Global.Player != null)
			Global.Player.transform.position += FixedHeight;
	}

	void PlayerStop(){
		CancelMoving (new Vector3(MoveToTarget.x, transform.position.y + FixedHeight.y, MoveToTarget.z));

	}

	void OnCollisionEnter(UnityEngine.Collision other){
		switch (other.gameObject.tag) {
		case "Moveable":
			if (Global.BePushedObj == null) {
				MoveSpeed = 2;
				Global.Player.transform.rotation = GameObject.Find ("GlobalScripts").GetComponent<PathController> ().FaceRotation;
				RotateDir = Global.Player.transform.rotation;

				CancelMoving (new Vector3 (CurrentFloor.transform.position.x, transform.position.y - 0.075f, CurrentFloor.transform.position.z));
				Global.BePushedObj = other.gameObject;
				Global.IsPushing = true;
				//transform.rotation = RotateDir;
				BoxPosY = other.transform.position.y;
				Global.BePushedObj.transform.parent = Global.Player.transform;

				if (RotateDir == Quaternion.Euler (0, 0, 0) || RotateDir == Quaternion.Euler (0, 180, 0)) {
					LockDirR = true;
					LockDirL = false;
				} else if (RotateDir == Quaternion.Euler (0, 90, 0) || RotateDir == Quaternion.Euler (0, -90, 0)) {
					LockDirR = false;
					LockDirL = true;
				}
			} else {
				CancelMoving (new Vector3 (CurrentFloor.transform.position.x, transform.position.y, CurrentFloor.transform.position.z));
			}
			break;

		case "Obstacle":
		case "EnemyWall":
		case "Bush":
			CancelMoving (new Vector3(CurrentFloor.transform.position.x, transform.position.y, CurrentFloor.transform.position.z));
			break;

		default:
			break;
		}

	}

	void OnCollisionStay(UnityEngine.Collision other){
		if (other.gameObject.tag == "Obstacle") 
		{
			
			CancelMoving (new Vector3(CurrentFloor.transform.position.x, transform.position.y, CurrentFloor.transform.position.z));

		}

		if (other.gameObject.tag == "Moveable") {
			if (Global.BePushedObj != null) {
				CancelMoving (new Vector3 (CurrentFloor.transform.position.x, transform.position.y + FixedHeight.y, CurrentFloor.transform.position.z));
			}
		}
	}

	void OnTriggerStay(Collider other){
		if(other.gameObject.tag == "Obstacle"){
			CancelMoving (new Vector3(CurrentFloor.transform.position.x, transform.position.y, CurrentFloor.transform.position.z));

		}

		/*if(other.transform.name == "Palace" && !isInside){
			//TheEntry = other.gameObject;
			OldCamPos = CameraController.CurrentCam.transform.position;
			OldCamView = CameraController.CamView;
			OldPlayerPos = Global.Player.transform.position;
			OldPlayerRot = GameObject.Find("GlobalScripts").GetComponent<PathController>().FaceRotation;

			//CameraController.CurrentCam.transform.position = GameObject.Find("InsideCam").transform.position;
			//CameraController.CurrentCam.transform.rotation = Quaternion.LookRotation(GameObject.Find("CameraTarget").transform.position - GameObject.Find("InsideCam").transform.position, GameObject.Find("CameraTarget").transform.up);
			//CameraController.CamView = 40;
			//Global.Player.transform.position = GameObject.Find("PalaceCenter").transform.position;
			//GameObject.Find("GlobalScripts").GetComponent<PathController>().FaceRotation = GameObject.Find("PalaceCenter").transform.rotation;
			inside = GameObject.Find("Event_Palace(open)").GetComponent<InsideMode>();
			Invoke("SetInside", 0.01f);
			isInside = true;
			//DoOnce = true;
		}*/
		
		/*if(other.transform.name == "Station" && !isInside){
			//TheEntry = other.gameObject;
			OldCamPos = CameraController.CurrentCam.transform.position;
			OldCamView = CameraController.CamView;
			OldPlayerPos = Global.Player.transform.position;
			OldPlayerRot = GameObject.Find("GlobalScripts").GetComponent<PathController>().FaceRotation;

			//CameraController.CurrentCam.transform.position = GameObject.Find("InsideCam").transform.position;
			//CameraController.CurrentCam.transform.rotation = Quaternion.LookRotation(GameObject.Find("CameraTarget").transform.position - GameObject.Find("InsideCam").transform.position, GameObject.Find("CameraTarget").transform.up);
			//CameraController.CamView = 40;
			//Global.Player.transform.position = GameObject.Find("StationCenter").transform.position;
			//GameObject.Find("GlobalScripts").GetComponent<PathController>().FaceRotation = GameObject.Find("StationCenter").transform.rotation;

			inside = GameObject.Find("Event_Station(open)").GetComponent<InsideMode>();
			Invoke("SetInside", 0.01f);
			isInside = true;
			//DoOnce = true;
		}*/

		/*if(other.transform.tag == "Exit" && isInside){
			isOut = true;
			//CameraController.CurrentCam.transform.position = OldCamPos;
			//CameraController.CamView = OldCamView;
			//CameraController.CurrentCam.transform.rotation = Quaternion.LookRotation(CameraController.CamTarget.transform.position - CameraController.CurrentCam.transform.position, CameraController.CamTarget.transform.up);
			Global.Player.transform.position = OldPlayerPos;
			GameObject.Find("GlobalScripts").GetComponent<PathController>().FaceRotation = OldPlayerRot * Quaternion.Euler(0, 180, 0);

			//TheEntry.GetComponent<Collider>().enabled = false;
			//Invoke("GoExit", 0.1f);

			CameraController.CurrentCam.transform.position = OldCamPos;
			CameraController.CamView = OldCamView;
			
			print("Exit");
			isInside = false;
			CameraController.CurrentCam.transform.LookAt (GameObject.Find("CamScript").GetComponent<CameraController> ().ScreenHeart.transform);
			//Global.Player.transform.position = new Vector3(-3, 6, 0);
			//DoOnce = false;
			//CancelInvoke();

		}*/
	}

	/*public void SetInside(){
		inside.enabled = true;
		inside.reset();
		//CancelInvoke();
		isInside = true;
	}*/

	/*public void GoExit(){

			//inside.setPlayer(Global.Player, inside.getOldPlayerPosition(), 1, false);
			//inside.setCamera(CameraController.CurrentCam, inside.getOldCamPosition(), inside.getOldCamView(), CameraController.CamTarget);
			CameraController.CurrentCam.transform.position = OldCamPos;
			CameraController.CamView = OldCamView;
			Global.Player.transform.position = new Vector3(3, 3, 0);

			//inside.setCamera(CameraController.CurrentCam, OldCamPos, OldCamView, CameraController.CamTarget);
			//inside.setPlayer(Global.Player, OldPlayerPos, 1, false);
			
			//inside.enabled = false;
			isInside = false;
			CameraController.CurrentCam.transform.LookAt (GameObject.Find("CamScript").GetComponent<CameraController> ().ScreenHeart.transform);
			CancelInvoke();
	}*/

	// 停止主角移動，並設定位置
	public static void CancelMoving(Vector3 NewPosition){
		Global.Player.transform.position = NewPosition;
		PathController.FollowPath = false;
		Global.PlayerMove = false;
		//GameObject.Find ("GlobalScripts").GetComponent<PathController> ().Reset ();
		GameObject[] AllFloors = GameObject.FindGameObjectsWithTag("BuildFloor");
		foreach(GameObject color in AllFloors){
			if(Global.Level != "Astar")
			GameObject.Find("GlobalScripts").GetComponent<PathController>().BeTouchedFloor.GetComponent<Renderer>().enabled = false;
		}
	}


}
