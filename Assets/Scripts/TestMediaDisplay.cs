using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MediaDisplay;

public class TestMediaDisplay : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Screen object in the scene")]
    public GameObject screenObject;
    [Tooltip("Camera object in the scene")]
    public GameObject cameraObject;
    [Tooltip("A test texture to display on the screen")]
    public Texture testTexture;

    [Header("Webcam")]
    [Tooltip("Option to use webcam as input")]
    public bool useWebcam = false;
    [Tooltip("Requested webcam dimensions")]
    public Vector2Int webcamDims = new Vector2Int(1280, 720);
    [Tooltip("Requested webcam framerate")]
    [Range(0, 60)]
    public int webcamFrameRate = 60;

    [Header("Toggle Key")]
    [Tooltip("Key to toggle between image and webcam feed")]
    public KeyCode toggleKey = KeyCode.Space;

    // The current texture displayed on the screen
    private Texture currentTexture;
    // Array of available webcam devices
    private WebCamDevice[] webcamDevices;
    // Texture to display live webcam video input
    private WebCamTexture webcamTexture;
    // Name of the currently selected webcam device
    private string currentWebcam;

    // Start is called before the first frame update
    void Start()
    {
        // Limit frame rate to avoid lag
        Application.targetFrameRate = 500;

        // Get the list of available webcam devices
        webcamDevices = WebCamTexture.devices;
        // If no webcam devices are available, disable the useWebcam option
        useWebcam = webcamDevices.Length > 0 ? useWebcam : false;
        currentWebcam = webcamDevices[0].name;

        // Initialize the webcam if available
        if (webcamDevices.Length > 0 && useWebcam)
        {
            bool webcamPlaying = MediaDisplayManager.InitializeWebcam(ref webcamTexture, currentWebcam, webcamDims, webcamFrameRate);
            useWebcam = webcamPlaying ? useWebcam : false;
        }

        // Update the screen texture based on the useWebcam option
        currentTexture = useWebcam ? webcamTexture : testTexture;
        if (useWebcam) Application.targetFrameRate = webcamFrameRate * 4;
        MediaDisplayManager.UpdateScreenTexture(screenObject, currentTexture, cameraObject, useWebcam);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle between image and webcam feed on key press
        if (Input.GetKeyUp(toggleKey))
        {
            useWebcam = !useWebcam;
            // If no webcam devices are available, disable the useWebcam option
            useWebcam = webcamDevices.Length > 0 ? useWebcam : false;
            // Initialize the webcam if available
            if (webcamDevices.Length > 0 && useWebcam)
            {
                bool webcamPlaying = MediaDisplayManager.InitializeWebcam(ref webcamTexture, currentWebcam, webcamDims, webcamFrameRate);
                useWebcam = webcamPlaying ? useWebcam : false;
            }
            //Disable webcam if playing
            else if (webcamTexture.isPlaying)
            {
                webcamTexture.Stop();
            }
            currentTexture = useWebcam ? webcamTexture : testTexture;
            if (useWebcam) Application.targetFrameRate = webcamFrameRate * 4;
            MediaDisplayManager.UpdateScreenTexture(screenObject, currentTexture, cameraObject, useWebcam);
        }
    }
}
