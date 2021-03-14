using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScreenshot : MonoBehaviour
{
    public string screenFilePrefix = "screenshot_";

    private int screenshotId = 0;

	void Start ()
    {
		
	}

	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.C))
        {
            ScreenCapture.CaptureScreenshot(screenFilePrefix + screenshotId.ToString() + ".png");
            screenshotId++;
        }
	}
}
