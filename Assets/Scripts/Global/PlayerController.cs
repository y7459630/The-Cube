using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static GameObject Player;

    void Awake(){
        Player = gameObject;
    }
}
