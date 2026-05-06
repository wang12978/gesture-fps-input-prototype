# gesture-fps-input-prototype
Unity FPS prototype controlled by real-time hand gesture recognition using Python, MediaPipe and OpenCV.
# Gesture FPS Input Prototype

This repository contains the source code for my CSC3094 dissertation project: **Real-Time Gesture Recognition Using 2D Camera Input**.

The project investigates whether static hand gestures can be used as an input method in a first-person shooter (FPS) prototype.

## Project Structure

- `UnityProject/` contains the Unity FPS prototype.
- `PythonGestureRecognition/` contains the Python gesture recognition module using MediaPipe and OpenCV.

## Technologies Used

- Unity 2022.3.62f3
- Python
- MediaPipe 0.10.21
- OpenCV 4.13.0.92
- UDP socket communication between Python and Unity

## How to Run

1. Open the Unity project in Unity 2022.3.62f3.
2. Install the Python dependencies:

```bash
pip install -r requirements.txt
