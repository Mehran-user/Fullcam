# 📷 Camera Viewer (Unity)

A lightweight fullscreen webcam viewer built with Unity UI.\
It displays a selected camera feed using `RawImage` and preserves aspect
ratio using `AspectRatioFitter`.

The app is configurable via a simple Linux-style text file.

------------------------------------------------------------------------

## ✨ Features

-   🎥 Live webcam preview
-   🖥️ Fullscreen UI-ready display
-   📐 Automatic aspect ratio handling
-   🔧 External config file (no recompilation needed)
-   🔁 Camera selection support
-   🪶 Lightweight and minimal

------------------------------------------------------------------------

## ⚙️ Configuration

The app reads a config file:

camera.conf

Located at:

-   Windows: `%AppData%/LocalLow/<Company>/<Product>/camera.conf`
-   Linux: `~/.config/unity3d/<Company>/<Product>/camera.conf`
-   Android: app-specific storage directory

------------------------------------------------------------------------

## 📄 Example config

CameraIndex = 0\
FitMode = ShrinkToFit

------------------------------------------------------------------------

## 🎛️ Settings

### CameraIndex

Selects which webcam device to use.

-   0 = first camera
-   1 = second camera, etc.

------------------------------------------------------------------------

### FitMode

Controls how the video fits the screen.

Options:

-   ShrinkToFit → Shows full image, may add black bars\
-   ZoomToFit → Fills screen, may crop edges

------------------------------------------------------------------------

## 🧠 How it works

Uses: - WebCamTexture for camera input - RawImage for rendering -
AspectRatioFitter for correct scaling

Config is read at startup from Unity's persistentDataPath.

------------------------------------------------------------------------

## 🚀 How to run

1.  Open project in Unity (6.2 recommended)
2.  Load the main scene
3.  Press Play or build the app
4.  Connect a webcam

------------------------------------------------------------------------

## 🔧 Notes

-   Config is auto-generated on first run
-   No cameras = no feed
-   Restart required for config changes (for now)

------------------------------------------------------------------------

## 🧃 Future ideas

-   Hot reload config
-   Camera switching keys
-   Mirror mode
-   FPS overlay
-   Screenshot hotkey
-   Multi-camera grid

------------------------------------------------------------------------

## 🪪 License

Do whatever you want with it 🚀
