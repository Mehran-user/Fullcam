using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CameraViewer : MonoBehaviour
{
    public RawImage rawImage;
    public AspectRatioFitter aspectFitter;

    private WebCamTexture webcamTexture;

    private int cameraIndex = 0;
    private FitMode fitMode = FitMode.ShrinkToFit;

    private string configPath;

    private enum FitMode
    {
        ShrinkToFit,
        ZoomToFit
    }

    void Start()
    {
        configPath = Path.Combine(Application.persistentDataPath, "camera.conf");
        LoadConfig();

        StartCamera();
    }

    void LoadConfig()
    {
        // fallback defaults
        cameraIndex = 0;
        fitMode = FitMode.ShrinkToFit;

        if (!File.Exists(configPath))
        {
            Debug.Log($"Config not found. Creating default at: {configPath}");
            File.WriteAllText(configPath,
@"CameraIndex = 0
FitMode = ShrinkToFit");
            return;
        }

        foreach (var line in File.ReadAllLines(configPath))
        {
            var trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                continue;

            var parts = trimmed.Split('=');
            if (parts.Length != 2) continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            if (key == "CameraIndex")
            {
                int.TryParse(value, out cameraIndex);
            }
            else if (key == "FitMode")
            {
                if (value == "ZoomToFit")
                    fitMode = FitMode.ZoomToFit;
                else
                    fitMode = FitMode.ShrinkToFit;
            }
        }
    }

    void StartCamera()
    {
        var devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No camera devices found.");
            return;
        }

        cameraIndex = Mathf.Clamp(cameraIndex, 0, devices.Length - 1);

        webcamTexture = new WebCamTexture(devices[cameraIndex].name);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();

        UpdateAspect();
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.width > 16)
        {
            UpdateAspect();
        }
    }

    void UpdateAspect()
    {
        float videoRatio = (float)webcamTexture.width / webcamTexture.height;
        aspectFitter.aspectRatio = videoRatio;

        if (fitMode == FitMode.ShrinkToFit)
        {
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        }
        else // ZoomToFit
        {
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}