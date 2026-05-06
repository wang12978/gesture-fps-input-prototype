using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 120f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        LockCursor(true);
    }

    void Update()
    {
        // ESC: Unlock the mouse
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        LockCursor(false);
        return;
    }

    // Left-click: Re-lock the mouse
    if (Cursor.lockState != CursorLockMode.Locked)
    {
        if (Input.GetMouseButtonDown(0))
            LockCursor(true);

        return;
    }

    float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
    float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -80f, 80f);

    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    public void ApplyGestureLook(float lookX, float lookY)
    {
        float deadzone = 0.12f;
        float maxTurnSpeed = 80f; // Maximum rotational speed (degrees per second)

        // ---------- Deadzone ----------
        if (Mathf.Abs(lookX) < deadzone)
            lookX = 0;
        else
            lookX = Mathf.Sign(lookX) * ((Mathf.Abs(lookX) - deadzone) / (1 - deadzone));

        if (Mathf.Abs(lookY) < deadzone)
            lookY = 0;
        else
            lookY = Mathf.Sign(lookY) * ((Mathf.Abs(lookY) - deadzone) / (1 - deadzone));

        // ---------- Nonlinearity (making the center more stable and the edges faster) ----------
        lookX = lookX * lookX * Mathf.Sign(lookX);
        lookY = lookY * lookY * Mathf.Sign(lookY);

        // ---------- Convert to speed ----------
        float turnX = lookX * maxTurnSpeed * Time.deltaTime;
        float turnY = -lookY * maxTurnSpeed * Time.deltaTime;

        // ---------- Up and down（pitch） ----------
        xRotation -= turnY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ---------- Left and right（yaw） ----------
        playerBody.Rotate(Vector3.up * turnX);
    }
}
