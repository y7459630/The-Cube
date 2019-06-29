using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour{
    public static GameObject CurrentCube;
    public static GameObject Builder;

    public static bool StopTouch;
    public static bool IsPreRotating;
    public static bool IsRotating;

    public GameObject GetBuilder;

    void Awake(){
        Builder = GetBuilder;
        Builder.SetActive(false);
    }

}
