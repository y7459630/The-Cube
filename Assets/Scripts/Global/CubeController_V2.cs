using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController_V2 : MonoBehaviour {

	public GameObject Arrow;
	public GameObject FormationArea;
	public GameObject CubeLeader;
	public GameObject CubeHome;
	public int FormationCount;

	private Ray ray;
	private RaycastHit hitinfo_Plane;
	private RaycastHit hitinfo2;
	private RaycastHit hitinfo_Cube;
	private GameObject RotatePlane;
	public static GameObject RotateCube;
	private string RotateDirection;
	private string RotateCode;
	private float MouseX;
	private float MouseY;
	private bool PreRotate;
	private int RX,RY,RZ;
	private Quaternion RotateTo90 = Quaternion.Euler(0,0,0);
	private Quaternion FixedRotation;
	private int mx,my;
	private float angle;
	private int ExtraChild;
	private bool IsOnRotatingCube = true;
	private float RotateSpeed = 15f;
	private bool IgnorePlayer;

	void Start () {
		//Global.IsCamCtrl = true;
	}


	void Update () {

		// 先別刪
		/*if (Global.OnCubeNum == CubeLeader.transform.transform.parent.GetComponent<FloorBuilder> ().FloorID)
			IsOnRotatingCube = true;
		else
			IsOnRotatingCube = false;*/

		// 先別刪
		/*if (Global.Level == "3" || Global.Level == "4") {
			if (RotateCube != null && RotateCube.transform.parent == GameObject.Find ("CubeHome_V3").transform) {
				CubeLeader = GameObject.Find ("CubeLeader_V3");
				CubeHome = GameObject.Find ("CubeHome_V3");
				FormationCount = 9;
			} else if (RotateCube != null && RotateCube.transform.parent == GameObject.Find ("CubeHome_V2").transform) {
				CubeLeader = GameObject.Find ("CubeLeader_V2");
				CubeHome = GameObject.Find ("CubeHome_V2");
				FormationCount = 4;
			}
		}*/

		MouseX = Input.GetAxis ("Mouse X") ;
		MouseY = Input.GetAxis ("Mouse Y") ;

		ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Layer 16 = BuildFloor.
		if ((Input.GetMouseButtonDown (1)) && Physics.Raycast (ray, out hitinfo_Plane, 100, 1 << 16) && Global.GameMode.Equals("PlayMode") && !Global.IsRotating && !Global.PlayerMove && !Global.IsPushing && !Global.IsCamCtrl && !Global.StopTouch) {
			Debug.DrawLine (Camera.main.transform.position, hitinfo_Plane.transform.position, Color.yellow, 0.1f, true);
			if (hitinfo_Plane.collider.gameObject != null) {
				RotatePlane = hitinfo_Plane.collider.gameObject;
			}
			Arrow.transform.position = RotatePlane.transform.position;
			Arrow.transform.rotation = RotatePlane.transform.rotation;


		} 


		if (Global.Level == "0") {
			if (Input.GetMouseButtonDown (1) && RotatePlane == null) {
				Global.IsCamCtrl = true;
			}

			if (Input.GetMouseButtonUp (1)) {
				Global.IsCamCtrl = false;
				RotatePlane = null;
			}
		} else {
			if (Input.GetMouseButtonDown (1) && RotatePlane == null) {
				Global.IsCamCtrl = true;
			}

			else if (!Input.GetMouseButton (1)) {
				Global.IsCamCtrl = false;
				RotatePlane = null;
			}
		}


		// Layer 9 = Cube.
		if ((Input.GetMouseButtonDown (1)) && Physics.Raycast (ray, out hitinfo_Cube, 100, 1 << 9) && Global.GameMode.Equals("PlayMode") && !Global.IsRotating && !Global.PlayerMove && !Global.IsPushing && !Global.IsCamCtrl && !Global.StopTouch) {
			RotateCube = hitinfo_Cube.collider.gameObject;
		}

		// Layer 12 = Arrow.
		if ((Input.GetMouseButton (1)) && Physics.Raycast (ray, out hitinfo2, 100, 1 << 12) && Global.GameMode.Equals("PlayMode") && !Global.IsRotating && !Global.PlayerMove && !Global.IsPushing && !Global.IsCamCtrl && !Global.StopTouch) {
			if(hitinfo2.collider.name == "arrow1")
				RotateDirection = "w";
			if(hitinfo2.collider.name == "arrow2")
				RotateDirection = "s";
			if(hitinfo2.collider.name == "arrow3")
				RotateDirection = "a";
			if(hitinfo2.collider.name == "arrow4")
				RotateDirection = "d";
			//CubesFormation (RotatePlane.name + RotateDirection);
			CubesFormation (RotatePlane.GetComponent<FloorInfo>().OriginUnderCube + RotatePlane.GetComponent<FloorInfo>().FloorID + RotateDirection);
			Arrow.transform.position = new Vector3 (100, 100, 100);
		}

		if (!Input.GetMouseButton (1)) {
			Arrow.transform.position = new Vector3 (100, 100, 100);
		}

		//----------------------------------------------------------


		if(PreRotate){
			Global.IsPreRotating = true;
			Global.IsCamCtrl = false;

			if ((Input.GetMouseButton (1)) && CubeLeader.transform.childCount == (FormationCount + ExtraChild) && !Global.IsRotating && !Global.PlayerMove && !Global.IsPushing && !Global.IsCamCtrl && !Global.StopTouch) {
				if(Global.Player !=null && Global.Player.GetComponent<Rigidbody> () && !IgnorePlayer)
					Global.Player.GetComponent<Rigidbody> ().useGravity = false;

				// 判斷是否中途轉向
				if(MouseY * my > 0)
					CubeLeader.transform.Rotate (RX * Mathf.Abs(MouseY) * RotateSpeed, RY * MouseX * -RotateSpeed, RZ * Mathf.Abs(MouseY) * RotateSpeed);
				else if(MouseY * my < 0)
					CubeLeader.transform.Rotate (RX * Mathf.Abs(MouseY) * -RotateSpeed, RY * MouseX * -RotateSpeed, RZ * Mathf.Abs(MouseY) * -RotateSpeed);
				else if(MouseX * mx > 0)
					CubeLeader.transform.Rotate (RX * Mathf.Abs(MouseX) * RotateSpeed, RY * MouseX * -RotateSpeed, RZ * Mathf.Abs(MouseX) * RotateSpeed);
				else if(MouseX * mx < 0)
					CubeLeader.transform.Rotate (RX * Mathf.Abs(MouseX) * -RotateSpeed, RY * MouseX * -RotateSpeed, RZ * Mathf.Abs(MouseX) * -RotateSpeed);

				// 依X軸轉動
				if (Mathf.Abs (RX) == 1) {
					if (CubeLeader.transform.eulerAngles.x > 45 && CubeLeader.transform.eulerAngles.x <= 90)
						RotateTo90 = Quaternion.Euler (90, 0, 0);
					if (CubeLeader.transform.eulerAngles.x >= 270 && CubeLeader.transform.eulerAngles.x < 315)
						RotateTo90 = Quaternion.Euler (270, 0, 0);

					// 因X軸的角度偵測很奇妙，所以用其他條件來修正
					if (CubeLeader.transform.eulerAngles.x > 315 || CubeLeader.transform.eulerAngles.x <= 45) {
						switch(RotateCode){
						case"V2R05":
						case"V2R06":
						case"V3R07":
						case"V3R08":
						case"V3R09":
						case"V4R09":
						case"V4R10":
						case"V4R11":
						case"V4R12":
							if ((RotateTo90.eulerAngles.x == 90 && MouseY * my > 0) || (RotateTo90.eulerAngles.x == 270 && MouseY * my < 0))
								RotateTo90 = Quaternion.Euler (180, 0, 0);
							if ((RotateTo90.eulerAngles.x == 90 && MouseY * my < 0) || (RotateTo90.eulerAngles.x == 270 && MouseY * my > 0))
								RotateTo90 = Quaternion.Euler (0, 0, 0);
							break;

						case"V2R11":
						case"V2R12":
						case"V3R16":
						case"V3R17":
						case"V3R18":
						case"V4R21":
						case"V4R22":
						case"V4R23":
						case"V4R24":
							if ((RotateTo90.eulerAngles.x == 90 && MouseY * my > 0) || (RotateTo90.eulerAngles.x == 270 && MouseY * my < 0))
								RotateTo90 = Quaternion.Euler (0, 0, 0);
							if ((RotateTo90.eulerAngles.x == 90 && MouseY * my < 0) || (RotateTo90.eulerAngles.x == 270 && MouseY * my > 0))
								RotateTo90 = Quaternion.Euler (180, 0, 0);
							break;
						}
					}
				}

				// 依Y軸轉動
				if (Mathf.Abs(RY) == 1) {
					if (CubeLeader.transform.eulerAngles.y > 315 || CubeLeader.transform.eulerAngles.y <= 45)
						RotateTo90 = Quaternion.Euler (0, 0, 0);
					if (CubeLeader.transform.eulerAngles.y > 45 && CubeLeader.transform.eulerAngles.y <= 135)
						RotateTo90 = Quaternion.Euler (0, 90, 0);
					if (CubeLeader.transform.eulerAngles.y > 135 && CubeLeader.transform.eulerAngles.y <= 225)
						RotateTo90 = Quaternion.Euler (0, 180, 0);
					if (CubeLeader.transform.eulerAngles.y > 225 && CubeLeader.transform.eulerAngles.y <= 315)
						RotateTo90 = Quaternion.Euler (0, 270, 0);
				}

				// 依Z軸轉動
				if (Mathf.Abs(RZ) == 1) {
					if (CubeLeader.transform.eulerAngles.z > 315 || CubeLeader.transform.eulerAngles.z <= 45)
						RotateTo90 = Quaternion.Euler (0, 0, 0);
					if (CubeLeader.transform.eulerAngles.z > 45 && CubeLeader.transform.eulerAngles.z <= 135)
						RotateTo90 = Quaternion.Euler (0, 0, 90);
					if (CubeLeader.transform.eulerAngles.z > 135 && CubeLeader.transform.eulerAngles.z <= 225)
						RotateTo90 = Quaternion.Euler (0, 0, 180);
					if (CubeLeader.transform.eulerAngles.z > 225 && CubeLeader.transform.eulerAngles.z <= 315)
						RotateTo90 = Quaternion.Euler (0, 0, 270);
				}
			}

			// 放開滑鼠之後的轉動
			if (!Input.GetMouseButton (1) || Global.IsRotating) {
				Global.IsRotating = true;
				CubeLeader.transform.rotation = Quaternion.Lerp (CubeLeader.transform.rotation, RotateTo90, 15f * Time.deltaTime);

			
				// 判斷轉動是否將結束
				if ( Mathf.Abs(CubeLeader.transform.eulerAngles.x - RotateTo90.eulerAngles.x) + 
					 Mathf.Abs(CubeLeader.transform.eulerAngles.y - RotateTo90.eulerAngles.y) + 
					 Mathf.Abs(CubeLeader.transform.eulerAngles.z - RotateTo90.eulerAngles.z) < 5f) {
					CubeLeader.transform.rotation = RotateTo90;


					// 結束轉動
					for(int i = 0 ; i < (FormationCount + ExtraChild) ; i++){
						if(CubeLeader.transform.childCount != 0)
						CubeLeader.transform.GetChild (0).transform.parent = CubeHome.transform;
					}
					RotateTo90 = Quaternion.Euler (0, 0, 0);
					CubeLeader.transform.rotation = Quaternion.Euler (0, 0, 0);


					RotateCode = null;
					RotateCube = null;
					RotatePlane = null;
					RotateDirection = null;
					mx = my = RX = RY = RZ = 0;
					if (Global.Player != null && GameObject.Find("PlayerHome") && Global.Player.GetComponent<Rigidbody> () && !IgnorePlayer) {
						Global.Player.transform.SetParent (GameObject.Find ("PlayerHome").transform);
						Global.Player.GetComponent<Rigidbody> ().useGravity = true;
					}
					ExtraChild = 0;
					Global.IsPreRotating = false;
					Global.IsRotating = false;
					PreRotate = false;
				}
			}
		}

	}

	// 偵測第一次轉動的滑鼠方向
	void MouseSetting(int conx, int cony){
		mx = conx;
		my = cony;
	}

	// 判斷應轉動的代碼
	void CubesFormation (string Rot)
	{
		MouseSetting (MouseX.CompareTo(0), MouseY.CompareTo(0));
		if (IsOnRotatingCube) {
			switch (Rot) {

			// New
			// Cube_V3
			case"Cube200Aw":
			case"Cube210Aw":
			case"Cube220Aw":
			case"Cube200Bw":
			case"Cube201Bw":
			case"Cube202Bw":
			case"Cube220Dw":
			case"Cube221Dw":
			case"Cube222Dw":
			case"Cube202Fs":
			case"Cube212Fs":
			case"Cube222Fs":
				if (Global.Player.transform.position.z > CubeLeader.transform.position.z - 1.5f || IgnorePlayer) {
					RotateCode = "V3R01";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"Cube100Aw":
			case"Cube110Aw":
			case"Cube120Aw":
			case"Cube100Bw":
			case"Cube101Bw":
			case"Cube102Bw":
			case"Cube120Dw":
			case"Cube121Dw":
			case"Cube122Dw":
			case"Cube102Fs":
			case"Cube112Fs":
			case"Cube122Fs":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z - 1.5f || Global.Player.transform.position.z > CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R02";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"Cube000Aw":
			case"Cube010Aw":
			case"Cube020Aw":
			case"Cube000Bw":
			case"Cube001Bw":
			case"Cube002Bw":
			case"Cube000Dw":
			case"Cube001Dw":
			case"Cube002Dw":
			case"Cube002Fs":
			case"Cube012Fs":
			case"Cube022Fs":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R03";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"Cube000Bd":
			case"Cube100Bd":
			case"Cube200Bd":
			case"Cube000Cw":
			case"Cube010Cw":
			case"Cube020Cw":
			case"Cube020Da":
			case"Cube120Da":
			case"Cube220Da":
			case"Cube200Es":
			case"Cube210Es":
			case"Cube220Es":

					RotateCode = "V3R04";
					RX = 0;
					RY = 1;
					RZ = 0;
				if (!IgnorePlayer) {
					Global.Player.transform.SetParent (CubeLeader.transform);
					ExtraChild = 1;
				}
				break;

			case"Cube001Bd":
			case"Cube101Bd":
			case"Cube201Bd":
			case"Cube001Cw":
			case"Cube011Cw":
			case"Cube021Cw":
			case"Cube021Da":
			case"Cube121Da":
			case"Cube221Da":
			case"Cube201Es":
			case"Cube211Es":
			case"Cube221Es":
				RotateCode = "V3R05";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"Cube002Bd":
			case"Cube102Bd":
			case"Cube202Bd":
			case"Cube002Cw":
			case"Cube012Cw":
			case"Cube022Cw":
			case"Cube022Da":
			case"Cube122Da":
			case"Cube222Da":
			case"Cube202Es":
			case"Cube212Es":
			case"Cube222Es":
				RotateCode = "V3R06";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"Cube020Ad":
			case"Cube120Ad":
			case"Cube220Ad":
			case"Cube020Cd":
			case"Cube021Cd":
			case"Cube022Cd":
			case"Cube220Ed":
			case"Cube221Ed":
			case"Cube222Ed":
			case"Cube022Fa":
			case"Cube122Fa":
			case"Cube222Fa":
				if (Global.Player.transform.position.x > CubeLeader.transform.position.x - 1.5f || IgnorePlayer) {
					RotateCode = "V3R07";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"Cube010Ad":
			case"Cube110Ad":
			case"Cube210Ad":
			case"Cube010Cd":
			case"Cube011Cd":
			case"Cube012Cd":
			case"Cube210Ed":
			case"Cube211Ed":
			case"Cube212Ed":
			case"Cube012Fa":
			case"Cube112Fa":
			case"Cube212Fa":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x - 1.5f || Global.Player.transform.position.x > CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R08";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"Cube000Ad":
			case"Cube100Ad":
			case"Cube200Ad":
			case"Cube000Cd":
			case"Cube001Cd":
			case"Cube002Cd":
			case"Cube200Ed":
			case"Cube201Ed":
			case"Cube202Ed":
			case"Cube002Fa":
			case"Cube102Fa":
			case"Cube202Fa":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R09";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"Cube000As":
			case"Cube010As":
			case"Cube020As":
			case"Cube000Bs":
			case"Cube001Bs":
			case"Cube002Bs":
			case"Cube000Ds":
			case"Cube001Ds":
			case"Cube002Ds":
			case"Cube002Fw":
			case"Cube012Fw":
			case"Cube022Fw":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R10";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"Cube100As":
			case"Cube110As":
			case"Cube120As":
			case"Cube100Bs":
			case"Cube101Bs":
			case"Cube102Bs":
			case"Cube100Ds":
			case"Cube101Ds":
			case"Cube102Ds":
			case"Cube102Fw":
			case"Cube112Fw":
			case"Cube122Fw":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z - 1.5f || Global.Player.transform.position.z > CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R11";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"Cube200As":
			case"Cube210As":
			case"Cube220As":
			case"Cube200Bs":
			case"Cube201Bs":
			case"Cube202Bs":
			case"Cube200Ds":
			case"Cube201Ds":
			case"Cube202Ds":
			case"Cube202Fw":
			case"Cube212Fw":
			case"Cube222Fw":
				if (Global.Player.transform.position.z > CubeLeader.transform.position.z - 1.5f || IgnorePlayer) {
					RotateCode = "V3R12";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"Cube002Ba":
			case"Cube102Ba":
			case"Cube202Ba":
			case"Cube002Cs":
			case"Cube012Cs":
			case"Cube022Cs":
			case"Cube022Dd":
			case"Cube122Dd":
			case"Cube222Dd":
			case"Cube202Ew":
			case"Cube212Ew":
			case"Cube222Ew":
				RotateCode = "V3R13";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"Cube001Ba":
			case"Cube101Ba":
			case"Cube201Ba":
			case"Cube001Cs":
			case"Cube011Cs":
			case"Cube021Cs":
			case"Cube021Dd":
			case"Cube121Dd":
			case"Cube221Dd":
			case"Cube201Ew":
			case"Cube211Ew":
			case"Cube221Ew":
				RotateCode = "V3R14";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"Cube000Ba":
			case"Cube100Ba":
			case"Cube200Ba":
			case"Cube000Cs":
			case"Cube010Cs":
			case"Cube020Cs":
			case"Cube020Dd":
			case"Cube120Dd":
			case"Cube220Dd":
			case"Cube200Ew":
			case"Cube210Ew":
			case"Cube220Ew":

					RotateCode = "V3R15";
					RX = 0;
					RY = 1;
					RZ = 0;
				if (!IgnorePlayer) {
					Global.Player.transform.SetParent (CubeLeader.transform);
					ExtraChild = 1;
				}
				break;

			case"Cube000Aa":
			case"Cube100Aa":
			case"Cube200Aa":
			case"Cube000Ca":
			case"Cube001Ca":
			case"Cube002Ca":
			case"Cube200Ea":
			case"Cube201Ea":
			case"Cube202Ea":
			case"Cube002Fd":
			case"Cube102Fd":
			case"Cube202Fd":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R16";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"Cube010Aa":
			case"Cube110Aa":
			case"Cube210Aa":
			case"Cube010Ca":
			case"Cube011Ca":
			case"Cube012Ca":
			case"Cube210Ea":
			case"Cube211Ea":
			case"Cube212Ea":
			case"Cube012Fd":
			case"Cube112Fd":
			case"Cube212Fd":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x - 1.5f || Global.Player.transform.position.x > CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R17";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"Cube020Aa":
			case"Cube120Aa":
			case"Cube220Aa":
			case"Cube020Ca":
			case"Cube021Ca":
			case"Cube022Ca":
			case"Cube220Ea":
			case"Cube221Ea":
			case"Cube222Ea":
			case"Cube022Fd":
			case"Cube122Fd":
			case"Cube222Fd":
				if (Global.Player.transform.position.x > CubeLeader.transform.position.x - 1.5f || IgnorePlayer) {
					RotateCode = "V3R18";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			/*
			case"V2A03w":
			case"V2A04w":
			case"V2B03w":
			case"V2B04w":
			case"V2D03w":
			case"V2D04w":
			case"V2F03s":
			case"V2F04s":
				if (Global.Player.transform.position.z > CubeLeader.transform.position.z || IgnorePlayer) {
					RotateCode = "V2R01";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"V2A01w":
			case"V2A02w":
			case"V2B01w":
			case"V2B02w":
			case"V2D01w":
			case"V2D02w":
			case"V2F01s":
			case"V2F02s":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z || IgnorePlayer) {
					RotateCode = "V2R02";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"V2B01d":
			case"V2B03d":
			case"V2C01w":
			case"V2C03w":
			case"V2D01a":
			case"V2D03a":
			case"V2E01s":
			case"V2E03s":
				RotateCode = "V2R03";
				RX = 0;
				RY = 1;
				RZ = 0;
				if (!IgnorePlayer) {
					Global.Player.transform.SetParent (CubeLeader.transform);
					ExtraChild = 1;
				}
				break;

			case"V2B02d":
			case"V2B04d":
			case"V2C02w":
			case"V2C04w":
			case"V2D02a":
			case"V2D04a":
			case"V2E02s":
			case"V2E04s":
				RotateCode = "V2R04";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"V2A02d":
			case"V2A04d":
			case"V2C03d":
			case"V2C04d":
			case"V2E03d":
			case"V2E04d":
			case"V2F02a":
			case"V2F04a":
				if (Global.Player.transform.position.x > CubeLeader.transform.position.x || IgnorePlayer) {
					RotateCode = "V2R05";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V2A01d":
			case"V2A03d":
			case"V2C01d":
			case"V2C02d":
			case"V2E01d":
			case"V2E02d":
			case"V2F01a":
			case"V2F03a":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x || IgnorePlayer) {
					RotateCode = "V2R06";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V2A01s":
			case"V2A02s":
			case"V2B01s":
			case"V2B02s":
			case"V2D01s":
			case"V2D02s":
			case"V2F01w":
			case"V2F02w":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z || IgnorePlayer) {
					RotateCode = "V2R07";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"V2A03s":
			case"V2A04s":
			case"V2B03s":
			case"V2B04s":
			case"V2D03s":
			case"V2D04s":
			case"V2F03w":
			case"V2F04w":
				if (Global.Player.transform.position.z > CubeLeader.transform.position.z || IgnorePlayer) {
					RotateCode = "V2R08";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"V2B02a":
			case"V2B04a":
			case"V2C02s":
			case"V2C04s":
			case"V2D02d":
			case"V2D04d":
			case"V2E02w":
			case"V2E04w":
				RotateCode = "V2R09";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"V2B01a":
			case"V2B03a":
			case"V2C01s":
			case"V2C03s":
			case"V2D01d":
			case"V2D03d":
			case"V2E01w":
			case"V2E03w":

					RotateCode = "V2R10";
					RX = 0;
					RY = 1;
					RZ = 0;
				if (!IgnorePlayer) {
					Global.Player.transform.SetParent (CubeLeader.transform);
					ExtraChild = 1;
				}
				break;

			case"V2A01a":
			case"V2A03a":
			case"V2C01a":
			case"V2C02a":
			case"V2E01a":
			case"V2E02a":
			case"V2F01d":
			case"V2F03d":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x || IgnorePlayer) {
					RotateCode = "V2R11";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V2A02a":
			case"V2A04a":
			case"V2C03a":
			case"V2C04a":
			case"V2E03a":
			case"V2E04a":
			case"V2F02d":
			case"V2F04d":
				if (Global.Player.transform.position.x > CubeLeader.transform.position.x || IgnorePlayer) {
					RotateCode = "V2R12";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			// Cube_V3
			case"V3A07w":
			case"V3A08w":
			case"V3A09w":
			case"V3B07w":
			case"V3B08w":
			case"V3B09w":
			case"V3D07w":
			case"V3D08w":
			case"V3D09w":
			case"V3F07s":
			case"V3F08s":
			case"V3F09s":
				if (Global.Player.transform.position.z > CubeLeader.transform.position.z - 1.5f || IgnorePlayer) {
					RotateCode = "V3R01";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"V3A04w":
			case"V3A05w":
			case"V3A06w":
			case"V3B04w":
			case"V3B05w":
			case"V3B06w":
			case"V3D04w":
			case"V3D05w":
			case"V3D06w":
			case"V3F04s":
			case"V3F05s":
			case"V3F06s":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z - 1.5f || Global.Player.transform.position.z > CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R02";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"V3A01w":
			case"V3A02w":
			case"V3A03w":
			case"V3B01w":
			case"V3B02w":
			case"V3B03w":
			case"V3D01w":
			case"V3D02w":
			case"V3D03w":
			case"V3F01s":
			case"V3F02s":
			case"V3F03s":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R03";
					RX = 0;
					RY = 0;
					RZ = 1;
				}
				break;

			case"V3B01d":
			case"V3B04d":
			case"V3B07d":
			case"V3C01w":
			case"V3C04w":
			case"V3C07w":
			case"V3D01a":
			case"V3D04a":
			case"V3D07a":
			case"V3E01s":
			case"V3E04s":
			case"V3E07s":

					RotateCode = "V3R04";
					RX = 0;
					RY = 1;
					RZ = 0;
				if (!IgnorePlayer) {
					Global.Player.transform.SetParent (CubeLeader.transform);
					ExtraChild = 1;
				}
				break;

			case"V3B02d":
			case"V3B05d":
			case"V3B08d":
			case"V3C02w":
			case"V3C05w":
			case"V3C08w":
			case"V3D02a":
			case"V3D05a":
			case"V3D08a":
			case"V3E02s":
			case"V3E05s":
			case"V3E08s":
				RotateCode = "V3R05";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"V3B03d":
			case"V3B06d":
			case"V3B09d":
			case"V3C03w":
			case"V3C06w":
			case"V3C09w":
			case"V3D03a":
			case"V3D06a":
			case"V3D09a":
			case"V3E03s":
			case"V3E06s":
			case"V3E09s":
				RotateCode = "V3R06";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"V3A03d":
			case"V3A06d":
			case"V3A09d":
			case"V3C07d":
			case"V3C08d":
			case"V3C09d":
			case"V3E07d":
			case"V3E08d":
			case"V3E09d":
			case"V3F03a":
			case"V3F06a":
			case"V3F09a":
				if (Global.Player.transform.position.x > CubeLeader.transform.position.x - 1.5f || IgnorePlayer) {
					RotateCode = "V3R07";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V3A02d":
			case"V3A05d":
			case"V3A08d":
			case"V3C04d":
			case"V3C05d":
			case"V3C06d":
			case"V3E04d":
			case"V3E05d":
			case"V3E06d":
			case"V3F02a":
			case"V3F05a":
			case"V3F08a":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x - 1.5f || Global.Player.transform.position.x > CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R08";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V3A01d":
			case"V3A04d":
			case"V3A07d":
			case"V3C01d":
			case"V3C02d":
			case"V3C03d":
			case"V3E01d":
			case"V3E02d":
			case"V3E03d":
			case"V3F01a":
			case"V3F04a":
			case"V3F07a":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R09";
					RX = 1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V3A01s":
			case"V3A02s":
			case"V3A03s":
			case"V3B01s":
			case"V3B02s":
			case"V3B03s":
			case"V3D01s":
			case"V3D02s":
			case"V3D03s":
			case"V3F01w":
			case"V3F02w":
			case"V3F03w":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R10";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"V3A04s":
			case"V3A05s":
			case"V3A06s":
			case"V3B04s":
			case"V3B05s":
			case"V3B06s":
			case"V3D04s":
			case"V3D05s":
			case"V3D06s":
			case"V3F04w":
			case"V3F05w":
			case"V3F06w":
				if (Global.Player.transform.position.z < CubeLeader.transform.position.z - 1.5f || Global.Player.transform.position.z > CubeLeader.transform.position.z + 1.5f || IgnorePlayer) {
					RotateCode = "V3R11";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"V3A07s":
			case"V3A08s":
			case"V3A09s":
			case"V3B07s":
			case"V3B08s":
			case"V3B09s":
			case"V3D07s":
			case"V3D08s":
			case"V3D09s":
			case"V3F07w":
			case"V3F08w":
			case"V3F09w":
				if (Global.Player.transform.position.z > CubeLeader.transform.position.z - 1.5f || IgnorePlayer) {
					RotateCode = "V3R12";
					RX = 0;
					RY = 0;
					RZ = -1;
				}
				break;

			case"V3B03a":
			case"V3B06a":
			case"V3B09a":
			case"V3C03s":
			case"V3C06s":
			case"V3C09s":
			case"V3D03d":
			case"V3D06d":
			case"V3D09d":
			case"V3E03w":
			case"V3E06w":
			case"V3E09w":
				RotateCode = "V3R13";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"V3B02a":
			case"V3B05a":
			case"V3B08a":
			case"V3C02s":
			case"V3C05s":
			case"V3C08s":
			case"V3D02d":
			case"V3D05d":
			case"V3D08d":
			case"V3E02w":
			case"V3E05w":
			case"V3E08w":
				RotateCode = "V3R14";
				RX = 0;
				RY = 1;
				RZ = 0;
				break;

			case"V3B01a":
			case"V3B04a":
			case"V3B07a":
			case"V3C01s":
			case"V3C04s":
			case"V3C07s":
			case"V3D01d":
			case"V3D04d":
			case"V3D07d":
			case"V3E01w":
			case"V3E04w":
			case"V3E07w":

					RotateCode = "V3R15";
					RX = 0;
					RY = 1;
					RZ = 0;
				if (!IgnorePlayer) {
					Global.Player.transform.SetParent (CubeLeader.transform);
					ExtraChild = 1;
				}
				break;

			case"V3A01a":
			case"V3A04a":
			case"V3A07a":
			case"V3C01a":
			case"V3C02a":
			case"V3C03a":
			case"V3E01a":
			case"V3E02a":
			case"V3E03a":
			case"V3F01d":
			case"V3F04d":
			case"V3F07d":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R16";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V3A02a":
			case"V3A05a":
			case"V3A08a":
			case"V3C04a":
			case"V3C05a":
			case"V3C06a":
			case"V3E04a":
			case"V3E05a":
			case"V3E06a":
			case"V3F02d":
			case"V3F05d":
			case"V3F08d":
				if (Global.Player.transform.position.x < CubeLeader.transform.position.x - 1.5f || Global.Player.transform.position.x > CubeLeader.transform.position.x + 1.5f || IgnorePlayer) {
					RotateCode = "V3R17";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			case"V3A03a":
			case"V3A06a":
			case"V3A09a":
			case"V3C07a":
			case"V3C08a":
			case"V3C09a":
			case"V3E07a":
			case"V3E08a":
			case"V3E09a":
			case"V3F03d":
			case"V3F06d":
			case"V3F09d":
				if (Global.Player.transform.position.x > CubeLeader.transform.position.x - 1.5f || IgnorePlayer) {
					RotateCode = "V3R18";
					RX = -1;
					RY = 0;
					RZ = 0;
				}
				break;

			// Cube_V4
			case"V4A13w":
			case"V4A14w":
			case"V4A15w":
			case"V4A16w":
			case"V4B13w":
			case"V4B14w":
			case"V4B15w":
			case"V4B16w":
			case"V4D13w":
			case"V4D14w":
			case"V4D15w":
			case"V4D16w":
			case"V4F13s":
			case"V4F14s":
			case"V4F15s":
			case"V4F16s":
				RotateCode = "V4R01";
				RX = 0;
				RY = 0;
				RZ = 1;
				break;

			case"V4A09w":
			case"V4A10w":
			case"V4A11w":
			case"V4A12w":
			case"V4B09w":
			case"V4B10w":
			case"V4B11w":
			case"V4B12w":
			case"V4D09w":
			case"V4D10w":
			case"V4D11w":
			case"V4D12w":
			case"V4F09s":
			case"V4F10s":
			case"V4F11s":
			case"V4F12s":
				RotateCode = "V4R02";
				RX = 0;
				RY = 0;
				RZ = 1;
			
				break;

			case"V4A05w":
			case"V4A06w":
			case"V4A07w":
			case"V4A08w":
			case"V4B05w":
			case"V4B06w":
			case"V4B07w":
			case"V4B08w":
			case"V4D05w":
			case"V4D06w":
			case"V4D07w":
			case"V4D08w":
			case"V4F05s":
			case"V4F06s":
			case"V4F07s":
			case"V4F08s":
				RotateCode = "V4R03";
				RX = 0;
				RY = 0;
				RZ = 1;
			
				break;

			case"V4A01w":
			case"V4A02w":
			case"V4A03w":
			case"V4A04w":
			case"V4B01w":
			case"V4B02w":
			case"V4B03w":
			case"V4B04w":
			case"V4D01w":
			case"V4D02w":
			case"V4D03w":
			case"V4D04w":
			case"V4F01s":
			case"V4F02s":
			case"V4F03s":
			case"V4F04s":
				RotateCode = "V4R04";
				RX = 0;
				RY = 0;
				RZ = 1;
			
				break;

			case"V4B01d":
			case"V4B05d":
			case"V4B09d":
			case"V4B13d":
			case"V4C01w":
			case"V4C05w":
			case"V4C09w":
			case"V4C13w":
			case"V4D01a":
			case"V4D05a":
			case"V4D09a":
			case"V4D13a":
			case"V4E01s":
			case"V4E05s":
			case"V4E09s":
			case"V4E13s":
				RotateCode = "V4R05";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4B02d":
			case"V4B06d":
			case"V4B10d":
			case"V4B14d":
			case"V4C02w":
			case"V4C06w":
			case"V4C10w":
			case"V4C14w":
			case"V4D02a":
			case"V4D06a":
			case"V4D10a":
			case"V4D14a":
			case"V4E02s":
			case"V4E06s":
			case"V4E10s":
			case"V4E14s":
				RotateCode = "V4R06";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4B03d":
			case"V4B07d":
			case"V4B11d":
			case"V4B15d":
			case"V4C03w":
			case"V4C07w":
			case"V4C11w":
			case"V4C15w":
			case"V4D03a":
			case"V4D07a":
			case"V4D11a":
			case"V4D15a":
			case"V4E03s":
			case"V4E07s":
			case"V4E11s":
			case"V4E15s":
				RotateCode = "V4R07";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4B04d":
			case"V4B08d":
			case"V4B12d":
			case"V4B16d":
			case"V4C04w":
			case"V4C08w":
			case"V4C12w":
			case"V4C16w":
			case"V4D04a":
			case"V4D08a":
			case"V4D12a":
			case"V4D16a":
			case"V4E04s":
			case"V4E08s":
			case"V4E12s":
			case"V4E16s":
				RotateCode = "V4R08";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4A04d":
			case"V4A08d":
			case"V4A12d":
			case"V4A16d":
			case"V4C13d":
			case"V4C14d":
			case"V4C15d":
			case"V4C16d":
			case"V4E13d":
			case"V4E14d":
			case"V4E15d":
			case"V4E16d":
			case"V4F04a":
			case"V4F08a":
			case"V4F12a":
			case"V4F16a":
				RotateCode = "V4R09";
				RX = 1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A03d":
			case"V4A07d":
			case"V4A11d":
			case"V4A15d":
			case"V4C09d":
			case"V4C10d":
			case"V4C11d":
			case"V4C12d":
			case"V4E09d":
			case"V4E10d":
			case"V4E11d":
			case"V4E12d":
			case"V4F03a":
			case"V4F07a":
			case"V4F11a":
			case"V4F15a":
				RotateCode = "V4R10";
				RX = 1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A02d":
			case"V4A06d":
			case"V4A10d":
			case"V4A14d":
			case"V4C05d":
			case"V4C06d":
			case"V4C07d":
			case"V4C08d":
			case"V4E05d":
			case"V4E06d":
			case"V4E07d":
			case"V4E08d":
			case"V4F02a":
			case"V4F06a":
			case"V4F10a":
			case"V4F14a":
				RotateCode = "V4R11";
				RX = 1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A01d":
			case"V4A05d":
			case"V4A09d":
			case"V4A13d":
			case"V4C01d":
			case"V4C02d":
			case"V4C03d":
			case"V4C04d":
			case"V4E01d":
			case"V4E02d":
			case"V4E03d":
			case"V4E04d":
			case"V4F01a":
			case"V4F05a":
			case"V4F09a":
			case"V4F13a":
				RotateCode = "V4R12";
				RX = 1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A01s":
			case"V4A02s":
			case"V4A03s":
			case"V4A04s":
			case"V4B01s":
			case"V4B02s":
			case"V4B03s":
			case"V4B04s":
			case"V4D01s":
			case"V4D02s":
			case"V4D03s":
			case"V4D04s":
			case"V4F01w":
			case"V4F02w":
			case"V4F03w":
			case"V4F04w":
				RotateCode = "V4R13";
				RX = 0;
				RY = 0;
				RZ = -1;
			
				break;

			case"V4A05s":
			case"V4A06s":
			case"V4A07s":
			case"V4A08s":
			case"V4B05s":
			case"V4B06s":
			case"V4B07s":
			case"V4B08s":
			case"V4D05s":
			case"V4D06s":
			case"V4D07s":
			case"V4D08s":
			case"V4F05w":
			case"V4F06w":
			case"V4F07w":
			case"V4F08w":
				RotateCode = "V4R14";
				RX = 0;
				RY = 0;
				RZ = -1;
			
				break;

			case"V4A09s":
			case"V4A10s":
			case"V4A11s":
			case"V4A12s":
			case"V4B09s":
			case"V4B10s":
			case"V4B11s":
			case"V4B12s":
			case"V4D09s":
			case"V4D10s":
			case"V4D11s":
			case"V4D12s":
			case"V4F09w":
			case"V4F10w":
			case"V4F11w":
			case"V4F12w":
				RotateCode = "V4R15";
				RX = 0;
				RY = 0;
				RZ = -1;
			
				break;

			case"V4A13s":
			case"V4A14s":
			case"V4A15s":
			case"V4A16s":
			case"V4B13s":
			case"V4B14s":
			case"V4B15s":
			case"V4B16s":
			case"V4D13s":
			case"V4D14s":
			case"V4D15s":
			case"V4D16s":
			case"V4F13w":
			case"V4F14w":
			case"V4F15w":
			case"V4F16w":
				RotateCode = "V4R16";
				RX = 0;
				RY = 0;
				RZ = -1;
			
				break;

			case"V4B04a":
			case"V4B08a":
			case"V4B12a":
			case"V4B16a":
			case"V4C04s":
			case"V4C08s":
			case"V4C12s":
			case"V4C16s":
			case"V4D04d":
			case"V4D08d":
			case"V4D12d":
			case"V4D16d":
			case"V4E04w":
			case"V4E08w":
			case"V4E12w":
			case"V4E16w":
				RotateCode = "V4R17";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4B03a":
			case"V4B07a":
			case"V4B11a":
			case"V4B15a":
			case"V4C03s":
			case"V4C07s":
			case"V4C11s":
			case"V4C15s":
			case"V4D03d":
			case"V4D07d":
			case"V4D11d":
			case"V4D15d":
			case"V4E03w":
			case"V4E07w":
			case"V4E11w":
			case"V4E15w":
				RotateCode = "V4R18";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4B02a":
			case"V4B06a":
			case"V4B10a":
			case"V4B14a":
			case"V4C02s":
			case"V4C06s":
			case"V4C10s":
			case"V4C14s":
			case"V4D02d":
			case"V4D06d":
			case"V4D10d":
			case"V4D14d":
			case"V4E02w":
			case"V4E06w":
			case"V4E10w":
			case"V4E14w":
				RotateCode = "V4R19";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4B01a":
			case"V4B05a":
			case"V4B09a":
			case"V4B13a":
			case"V4C01s":
			case"V4C05s":
			case"V4C09s":
			case"V4C13s":
			case"V4D01d":
			case"V4D05d":
			case"V4D09d":
			case"V4D13d":
			case"V4E01w":
			case"V4E05w":
			case"V4E09w":
			case"V4E13w":
				RotateCode = "V4R20";
				RX = 0;
				RY = 1;
				RZ = 0;
			
				break;

			case"V4A01a":
			case"V4A05a":
			case"V4A09a":
			case"V4A13a":
			case"V4C01a":
			case"V4C02a":
			case"V4C03a":
			case"V4C04a":
			case"V4E01a":
			case"V4E02a":
			case"V4E03a":
			case"V4E04a":
			case"V4F01d":
			case"V4F05d":
			case"V4F09d":
			case"V4F13d":
				RotateCode = "V4R21";
				RX = -1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A02a":
			case"V4A06a":
			case"V4A10a":
			case"V4A14a":
			case"V4C05a":
			case"V4C06a":
			case"V4C07a":
			case"V4C08a":
			case"V4E05a":
			case"V4E06a":
			case"V4E07a":
			case"V4E08a":
			case"V4F02d":
			case"V4F06d":
			case"V4F10d":
			case"V4F14d":
				RotateCode = "V4R22";
				RX = -1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A03a":
			case"V4A07a":
			case"V4A11a":
			case"V4A15a":
			case"V4C09a":
			case"V4C10a":
			case"V4C11a":
			case"V4C12a":
			case"V4E09a":
			case"V4E10a":
			case"V4E11a":
			case"V4E12a":
			case"V4F03d":
			case"V4F07d":
			case"V4F11d":
			case"V4F15d":
				RotateCode = "V4R23";
				RX = -1;
				RY = 0;
				RZ = 0;
			
				break;

			case"V4A04a":
			case"V4A08a":
			case"V4A12a":
			case"V4A16a":
			case"V4C13a":
			case"V4C14a":
			case"V4C15a":
			case"V4C16a":
			case"V4E13a":
			case"V4E14a":
			case"V4E15a":
			case"V4E16a":
			case"V4F04d":
			case"V4F08d":
			case"V4F12d":
			case"V4F16d":
				RotateCode = "V4R24";
				RX = -1;
				RY = 0;
				RZ = 0;
			
				break;*/

			}
		}

		switch (RotateCode) {

		case"V2R01":
		case"V2R02":
		case"V2R07":
		case"V2R08":
		case"V3R01":
		case"V3R02":
		case"V3R03":
		case"V3R10":
		case"V3R11":
		case"V3R12":
		case"V4R01":
		case"V4R02":
		case"V4R03":
		case"V4R04":
		case"V4R13":
		case"V4R14":
		case"V4R15":
		case"V4R16":
			if (RotateCube != null) {
				FormationArea.transform.position = RotateCube.transform.position;
				FormationArea.transform.rotation = Quaternion.Euler (90, 0, 0);
			}
			break;

		case"V2R03":
		case"V2R04":
		case"V2R09":
		case"V2R10":
		case"V3R04":
		case"V3R05":
		case"V3R06":
		case"V3R13":
		case"V3R14":
		case"V3R15":
		case"V4R05":
		case"V4R06":
		case"V4R07":
		case"V4R08":
		case"V4R17":
		case"V4R18":
		case"V4R19":
		case"V4R20":
			if (RotateCube != null) {
				FormationArea.transform.position = RotateCube.transform.position;
				FormationArea.transform.rotation = Quaternion.Euler (0, 0, 0);
			}
			break;

		case"V2R05":
		case"V2R06":
		case"V2R11":
		case"V2R12":
		case"V3R07":
		case"V3R08":
		case"V3R09":
		case"V3R16":
		case"V3R17":
		case"V3R18":
		case"V4R09":
		case"V4R10":
		case"V4R11":
		case"V4R12":
		case"V4R21":
		case"V4R22":
		case"V4R23":
		case"V4R24":
			if (RotateCube != null) {
				FormationArea.transform.position = RotateCube.transform.position;
				FormationArea.transform.rotation = Quaternion.Euler (0, 0, -90);
			}
			break;
		}

		PreRotate = true;


	}

}
