﻿using UnityEngine;
using System.Collections;

public class SystemCommands : MonoBehaviour 
{
	// Update is called once per frame
	void Update () 
  {
    if (Input.GetKey("escape"))
        Application.Quit();
    
    if (Input.GetKey("p"))
      Application.CaptureScreenshot("Screenshot.png");
	}
}
