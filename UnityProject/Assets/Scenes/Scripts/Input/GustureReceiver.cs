using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using System.Diagnostics;

public class GestureReceiver : MonoBehaviour
{
    public int listenPort = 5055;

    private UdpClient udpClient;
    private Thread receiveThread;

    private volatile bool shootRequested = false;
    private bool running = false;

    private float gestureLookX = 0f;
    private float gestureLookY = 0f;
    private readonly object dataLock = new object();
    private bool hasGestureLook = false;
    private long lastLookReceiveMs = 0;
    public float gestureTimeout = 0.3f;

    void Start()
    {
        try
        {
            udpClient = new UdpClient(listenPort);
            running = true;

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            UnityEngine.Debug.Log("GestureReceiver started on port " + listenPort);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Failed to start UDP listener: " + e.Message);
        }
    }

    void Update()
    {
        float lookX;
        float lookY;
        bool useGesture;

        lock (dataLock)
        {
            double elapsedSeconds = (Stopwatch.GetTimestamp() - lastLookReceiveMs) / (double)Stopwatch.Frequency;
            useGesture = hasGestureLook && elapsedSeconds < gestureTimeout;
            lookX = gestureLookX;
            lookY = gestureLookY;
        }

        PlayerLook playerLook = FindObjectOfType<PlayerLook>();
        if (playerLook != null && useGesture)
        {
            playerLook.ApplyGestureLook(lookX, lookY);
        }

        if (shootRequested)
        {
            shootRequested = false;

            
            UnityEngine.Debug.Log("Received SHOOT from Python");

            // Find GunRaycast and call ShootByGesture
            GunRaycast gun = FindObjectOfType<GunRaycast>();
            if (gun != null)
            {
                gun.ShootByGesture();
            }
        }
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        while (running)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data).Trim();

                if (message == "SHOOT")
                {
                    shootRequested = true;
                }
                else if (message.StartsWith("LOOK:"))
                {
                    string[] parts = message.Split(':');

                    if (parts.Length == 3)
                    {
                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);

                        lock (dataLock)
                        {
                            gestureLookX = x;
                            gestureLookY = y;
                            hasGestureLook = true;
                            lastLookReceiveMs = Stopwatch.GetTimestamp();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (running)
                {
                    UnityEngine.Debug.LogWarning("UDP receive error: " + e.Message);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        running = false;

        try
        {
            udpClient?.Close();
        }
        catch { }

        try
        {
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Abort();
            }
        }
        catch { }
    }
}