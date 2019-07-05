using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public GameObject Cam;
	public GameObject CamObj;
	public GameObject ScreenHeart;
	public GameObject RotateGoal;

	public static GameObject CurrentHeart;
	public static GameObject CurrentCam;
	public static float CamView;
	public static bool IsCamRotating;

	private Vector3 CamToScreenHeartOrigin;
	private Vector3 CamToScreenHeart;
	private Vector3 CamOriginPos;
	//private Quaternion CamOriginRot;
	private int viewTime = 9;
	private float mouseX, mouseY;
	private float distance;
	private float rotateSpeed = 0.4f;
	private bool isCamRotating;
	private bool isFollowing;

	void Awake () {
		CurrentCam = Cam;
		CurrentHeart = ScreenHeart;
		CamToScreenHeartOrigin = CamObj.transform.position - ScreenHeart.transform.position;
		CamView = Camera.main.fieldOfView;
		RotateGoal.transform.position = Cam.transform.position;
		Cam.transform.rotation = Quaternion.LookRotation (CamObj.transform.position - CamToScreenHeartOrigin - Cam.transform.position, ScreenHeart.transform.up);
		CamOriginPos = Cam.transform.position;
		//CamOriginRot = Cam.transform.rotation;
	}
	

	void Update () {

		mouseX = Input.GetAxis ("Mouse X") ;
		mouseY = Input.GetAxis ("Mouse Y") ;
		CamToScreenHeart = CamObj.transform.position - ScreenHeart.transform.position;
		distance = Vector3.Distance (Cam.transform.position, ScreenHeart.transform.position);


		if (!Global.IsPreRotating && !Global.IsRotating) {
			follow (25);
			checkRightClick ();
			if (Vector3.Distance (Cam.transform.position, RotateGoal.transform.position) > 0.1f) {
				Cam.transform.position = Vector3.Lerp (Cam.transform.position, RotateGoal.transform.position, rotateSpeed);
				Cam.transform.rotation = Quaternion.LookRotation (CamObj.transform.position - CamToScreenHeartOrigin - Cam.transform.position, ScreenHeart.transform.up);
			}
		}
		if(CamView != Camera.main.fieldOfView)
			Camera.main.fieldOfView -= (Camera.main.fieldOfView - CamView) / viewTime;
		if (Input.GetAxis ("Mouse ScrollWheel") != 0)
			zoom ();

	}

	public void checkRightClick(){
		if (Input.GetMouseButtonDown (1)){
			rotateSpeed /= 1.2f;
		}

		if (Input.GetMouseButton (1) && !Global.IsPreRotating && !Global.IsRotating && !Global.StopTouch)
			rotate ();
		else if (Input.GetMouseButtonUp (1))
			rotateSpeed *= 1.2f;
	}

	public void follow(int time){
		CamObj.transform.position += ((ScreenHeart.transform.position + CamToScreenHeartOrigin) - CamObj.transform.position) / time;
	}

	public void rotate(){


		// 左右
		if (mouseX > 0) {
			RotateGoal.transform.Translate (-120 * Time.deltaTime, 0, 0);
			RotateGoal.transform.position -= Vector3.forward * Time.deltaTime;
			RotateGoal.transform.rotation = Quaternion.LookRotation (CamObj.transform.position - CamToScreenHeartOrigin - RotateGoal.transform.position, ScreenHeart.transform.up);
		}
		if (mouseX < 0) {
			RotateGoal.transform.Translate (120 * Time.deltaTime, 0, 0);
			RotateGoal.transform.position += Vector3.forward * Time.deltaTime;
			RotateGoal.transform.rotation = Quaternion.LookRotation (CamObj.transform.position - CamToScreenHeartOrigin - RotateGoal.transform.position, ScreenHeart.transform.up);
		}

		// 上下
		if (mouseY > 0 && (RotateGoal.transform.position - ScreenHeart.transform.position).y > -45) {
			RotateGoal.transform.Translate (0, -120 * Time.deltaTime, 0);
			RotateGoal.transform.rotation = Quaternion.LookRotation (CamObj.transform.position - CamToScreenHeartOrigin - RotateGoal.transform.position, ScreenHeart.transform.up);
		}
		if (mouseY < 0 && (RotateGoal.transform.position - ScreenHeart.transform.position).y < 45) {
			RotateGoal.transform.Translate (0, 120 * Time.deltaTime, 0);
			RotateGoal.transform.rotation = Quaternion.LookRotation (CamObj.transform.position - CamToScreenHeartOrigin - RotateGoal.transform.position, ScreenHeart.transform.up);
		}

		// 抵銷攝影機旋轉的離心力
		if (distance > 50)
			RotateGoal.transform.position = Vector3.Lerp (RotateGoal.transform.position, ScreenHeart.transform.position, 0.01f);
	}

	public void zoom(){
		if (!Global.StopTouch && !Global.IsPreRotating && !Global.IsRotating) {
			if (Input.GetAxis ("Mouse ScrollWheel") < 0 && Camera.main.fieldOfView < 25) {
				CamView += 2f;
				CamView = Mathf.Clamp (CamView, 15, 25);
			} else if (Input.GetAxis ("Mouse ScrollWheel") > 0 && Camera.main.fieldOfView > 15) {
				CamView -= 2f;
				CamView = Mathf.Clamp (CamView, 15, 25);
			}
		}
	}
}
