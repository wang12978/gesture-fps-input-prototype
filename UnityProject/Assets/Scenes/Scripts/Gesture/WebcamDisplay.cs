using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class WebcamDisplay : MonoBehaviour
{
    [Header("Assign a RawImage in the Inspector")]
    public RawImage targetRawImage;

    [Header("Optional: leave empty to use default camera")]
    public string deviceName = "";

    [Header("Optional settings")]
    public int requestedWidth = 2560;
    public int requestedHeight = 1440;
    public int requestedFPS = 120;

    private WebCamTexture camTexture;

    void Start()
    {
        if (targetRawImage == null)
        {
            Debug.LogError("WebcamDisplay: targetRawImage is not assigned!");
            return;
        }

        // List cameras (helpful for debugging)
        foreach (var d in WebCamTexture.devices)
        {
            Debug.Log("Camera found: " + d.name);
        }

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No camera devices found. Check OS camera permissions.");
            return;
        }

        // Choose camera
        string chosen = deviceName;
        if (string.IsNullOrEmpty(chosen))
        {
            chosen = WebCamTexture.devices[0].name; // default first camera
        }

        camTexture = new WebCamTexture(chosen, requestedWidth, requestedHeight, requestedFPS);
        targetRawImage.texture = camTexture;
        camTexture.Play();
    }

    void OnDisable()
    {
        if (camTexture != null && camTexture.isPlaying)
            camTexture.Stop();
    }
}
