using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MediaDisplay;

public class TestMediaDisplay : MonoBehaviour
{
    // Scene components and settings
    [Header("Scene")]
    [Tooltip("Screen object in the scene")]
    public GameObject screenObject;
    [Tooltip("Camera object in the scene")]
    public GameObject cameraObject;
    [Tooltip("A test texture to display on the screen")]
    public Texture testTexture;

    // Webcam settings
    [Header("Webcam")]
    [Tooltip("Option to use webcam as input")]
    public bool useWebcam = false;
    [Tooltip("Requested webcam dimensions")]
    public Vector2Int webcamDims = new Vector2Int(1280, 720);
    [Tooltip("Requested webcam framerate")]
    [Range(0, 60)]
    public int webcamFrameRate = 60;

    // Toggle key settings
    [Header("Toggle Key")]
    [Tooltip("Key to toggle between image and webcam feed")]
    public KeyCode toggleKey = KeyCode.Space;

    // Private variables
    private Texture currentTexture;
    private WebCamDevice[] webcamDevices;
    private WebCamTexture webcamTexture;
    private string currentWebcam;

    // Subscribe to the texture change event
    private void OnEnable()
    {
        TextureChangeEvent.OnMainTextureChanged += HandleMainTextureChanged;
    }

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

    // Update the screen with the current texture (image or webcam feed)
    public void UpdateDisplay()
    {
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

        StartCoroutine(UpdateScreenTextureAsync());
    }

    // Coroutine to update the screen texture asynchronously
    IEnumerator UpdateScreenTextureAsync()
    {
        // Wait until the webcamTexture is ready
        while (webcamTexture.isPlaying && webcamTexture.width <= 16)
        {
            yield return null;
        }

        // Update the screen texture with the current texture (image or webcam feed)
        MediaDisplayManager.UpdateScreenTexture(screenObject, currentTexture, cameraObject, useWebcam);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle between image and webcam feed on key press
        if (Input.GetKeyUp(toggleKey))
        {
            useWebcam = !useWebcam;
            UpdateDisplay(); // Call the function to update the display
        }
    }

    // Handle the texture change event
    private void HandleMainTextureChanged(Material material)
    {
        // Update the current texture
        currentTexture = material.mainTexture;
        // If the new main texture is different from the webcam texture and the webcam is playing, stop the webcam
        if (webcamTexture && material.mainTexture != webcamTexture && webcamTexture.isPlaying)
        {
            useWebcam = false;
            webcamTexture.Stop();
        }
    }

    // Unsubscribe from the texture change event when the script is disabled
    private void OnDisable()
    {
        TextureChangeEvent.OnMainTextureChanged -= HandleMainTextureChanged;
    }
}
