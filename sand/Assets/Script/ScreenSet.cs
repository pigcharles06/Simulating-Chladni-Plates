using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        if(screenHeight+screenWidth !=2000)
            Screen.SetResolution(1280, 720, false);
    }
}
