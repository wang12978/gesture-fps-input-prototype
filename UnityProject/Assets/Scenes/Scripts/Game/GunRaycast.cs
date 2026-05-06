using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRaycast : MonoBehaviour
{
    public float range = 50f;

    [Header("Visual tracer")]
    public LineRenderer tracer;
    public float tracerTime = 0.05f;

    [Header("Muzzle (where the tracer starts)")]
    public Transform muzzle;

    void Start()
    {
        if (tracer != null) tracer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Shoot()
    {
        // 1) Hit judgment: Emit a ray from the center of the screen (in the crosshair direction)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 hitPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            hitPoint = hit.point;
            Debug.Log("Hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Target"))
            {
                GameStatsUI stats = FindObjectOfType<GameStatsUI>();
                if (stats != null)
                {
                    stats.AddTargetHit();
                }
                Destroy(hit.collider.gameObject);
            }
        }

        // Trajectory display: Draw from the muzzle position to the point of hit
        if (tracer != null && muzzle != null)
        {
            StartCoroutine(ShowTracer(muzzle.position, hitPoint));
        }
    }

    IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        tracer.SetPosition(0, start);
        tracer.SetPosition(1, end);

        tracer.enabled = true;
        yield return new WaitForSeconds(tracerTime);
        tracer.enabled = false;
    }

    public void ShootByGesture()
    {
        Shoot();
    }
}
