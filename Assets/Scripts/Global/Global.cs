using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour{
    public static GameObject CurrentCube;
    public static GameObject Builder;
    public static GameObject Player;
    public static GameObject BeTouchedObj;
    public static GameObject BePushedObj;
    public static GameObject GlobalObj;

    public static bool StopTouch;
    public static bool IsPreRotating;
    public static bool IsRotating;
    public static bool IsPushing;
    public static bool PlayerMove;

    public static float StartTime;

    public static string Level;
    public static string GameMode;

    public GameObject GetBuilder;

    void Awake(){
        StartTime = Time.time;
        GlobalObj = gameObject;
        Player = GameObject.FindGameObjectWithTag("Player");
        Builder = GetBuilder;
        Builder.SetActive(false);
        GameMode = "EditMode";
    }

}
