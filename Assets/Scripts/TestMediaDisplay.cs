using System.Collections;
using UnityEngine;
using MediaDisplay;

public class TestMediaDisplay : MonoBehaviour
{
    // Scene components and settings
    [Header("Scene")]
    [SerializeField] private GameObject screenObject;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private Texture testTexture;

    // Webcam settings
    [Header("Webcam")]
    [SerializeField] private bool useWebcam = false;
    [SerializeField] private Vector2Int webcamDims = new Vector2Int(1280, 720);
    [SerializeField, Range(0, 60)] private int webcamFrameRate = 60;

    // Toggle key settings
    [Header("Toggle Key")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Space;

    private Texture currentTexture;
    private WebCamDevice[] webcamDevices;
    private WebCamTexture webcamTexture;
    private string currentWebcam;

    // Subscribe to the texture change event
    private void OnEnable()
    {
        TextureChangeEvent.OnMainTextureChanged += HandleMainTextureChanged;
    }

    // Called when the script instance is being loaded.
    private void Start()
    {
        Initialize();
        UpdateDisplay();
    }

    // Initializes the application's target frame rate and configures the webcam devices.
    private void Initialize()
    {
        // Set the application's target frame rate to 500 to reduce lag.
        Application.targetFrameRate = 500;
        // Get the list of available webcam devices.
        webcamDevices = WebCamTexture.devices;
        // If no webcam devices are available, disable the useWebcam option.
        useWebcam = webcamDevices.Length > 0 ? useWebcam : false;
        // Set the current webcam device to the first available device, if any.
        currentWebcam = useWebcam ? webcamDevices[0].name : "";
    }

    // Updates the display with the current texture (either a test texture or the webcam feed).
    public void UpdateDisplay()
    {
        // Set up the webcam if necessary.
        SetupWebcam();
        // Update the current texture based on the useWebcam option.
        UpdateCurrentTexture();
        // Start a coroutine to asynchronously update the screen texture.
        StartCoroutine(UpdateScreenTextureAsync());
    }

    // Sets up the webcam if the useWebcam option is enabled.
    private void SetupWebcam()
    {
        // If there are no webcam devices, return immediately.
        if (webcamDevices.Length == 0) return;

        // If the useWebcam option is enabled, initialize the webcam.
        if (useWebcam)
        {
            // Initialize the webcam and check if it started playing.
            bool webcamPlaying = MediaDisplayManager.InitializeWebcam(ref webcamTexture, currentWebcam, webcamDims, webcamFrameRate);
            // If the webcam failed to start playing, disable the useWebcam option.
            useWebcam = webcamPlaying ? useWebcam : false;
        }
        // If the useWebcam option is disabled and the webcam texture is playing, stop the webcam.
        else if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }

    // Updates the current texture and target frame rate based on the useWebcam option.
    private void UpdateCurrentTexture()
    {
        // Set the current texture to the webcam texture if useWebcam is enabled, otherwise use the test texture.
        currentTexture = useWebcam ? webcamTexture : testTexture;
        // Set the target frame rate based on whether the webcam is being used or not.
        Application.targetFrameRate = useWebcam ? webcamFrameRate * 4 : 500;
    }

    // Coroutine to update the screen texture asynchronously.
    IEnumerator UpdateScreenTextureAsync()
    {
        // Wait until the webcamTexture is ready if useWebcam is enabled.
        while (useWebcam && webcamTexture.isPlaying && webcamTexture.width <= 16)
        {
            yield return null;
        }

        // Update the screen texture with the current texture (image or webcam feed).
        MediaDisplayManager.UpdateScreenTexture(screenObject, currentTexture, cameraObject, useWebcam);
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

    // Handle the texture change event.
    private void HandleMainTextureChanged(Material material)
    {
        // Update the current texture.
        currentTexture = material.mainTexture;
        // If the new main texture is different from the webcam texture and the webcam is playing, stop the webcam.
        if (webcamTexture && material.mainTexture != webcamTexture && webcamTexture.isPlaying)
        {
            useWebcam = false;
            webcamTexture.Stop();
        }
    }

    // Unsubscribe from the texture change event when the script is disabled.
    private void OnDisable()
    {
        TextureChangeEvent.OnMainTextureChanged -= HandleMainTextureChanged;
    }

}
