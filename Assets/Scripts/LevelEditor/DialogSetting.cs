using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSetting : MonoBehaviour{

    public Text Word;

    public void SetText(Text EnterText){
        Word.text = EnterText.text;
    }
}
