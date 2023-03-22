using System.Collections;
using UnityEngine;
using MediaDisplay;

public class TestMediaDisplay : BaseScreenManager
{
    // Called when the script instance is being loaded.
    private void Start()
    {
        Initialize();
        UpdateDisplay();
    }

    // Update is called once per frame.
    private void Update()
    {
        // Toggle between image and webcam feed on key press.
        if (Input.GetKeyUp(toggleKey))
        {
            useWebcam = !useWebcam;
            UpdateDisplay(); // Call the function to update the display.
        }
    }
}
