using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour{
    public bool UseFadeIn;
    public float FadeSpeed = 2.0f;
    private Image FadeImage;

    void Awake(){
        FadeImage = GetComponent<Image>();
        if(UseFadeIn){
            FadeImage.enabled = true;
            Global.StopTouch = true;
        }
        
    }

    void FixedUpdate(){
        if(Time.time - Global.StartTime >= 1 && UseFadeIn)
            FadeIn();
    }

    public void FadeIn(){
        FadeImage.color -= new Color(0, 0, 0, FadeSpeed * 0.01f);
        if(FadeImage.color.a <= 0){
            UseFadeIn = false;
            Global.StopTouch = false;
        }
    }
}
