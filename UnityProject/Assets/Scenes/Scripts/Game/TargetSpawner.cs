using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject targetPrefab;

    [Header("Spawn Settings")]
    public int maxTargets = 5;
    public float spawnInterval = 2f;

    [Header("Spawn Area (world coords)")]
    public Vector3 areaCenter = new Vector3(0f, 1f, 12f);
    public Vector3 areaSize = new Vector3(12f, 2f, 6f); // x wide, y high, z deep

    [Header("Anti-overlap")]
    public float minDistance = 1.5f;   // The minimum distance between targets
    public int maxAttempts = 20;       // The maximum number of attempts generated each time

    private readonly List<GameObject> aliveTargets = new List<GameObject>();
    private float timer = 0f;

    void Update()
    {
        // Clear the references that have been destroyed
        aliveTargets.RemoveAll(t => t == null);

        timer += Time.deltaTime;

        if (aliveTargets.Count < maxTargets && timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnOne();
        }
    }

    void TrySpawnOne()
    {
        if (targetPrefab == null) return;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 pos = GetRandomPointInArea();

            if (IsFarEnough(pos))
            {
                GameObject t = Instantiate(targetPrefab, pos, Quaternion.identity);
                aliveTargets.Add(t);
                return;
            }
        }

        
    }

    Vector3 GetRandomPointInArea()
    {
        Vector3 half = areaSize * 0.5f;

        float x = Random.Range(areaCenter.x - half.x, areaCenter.x + half.x);
        float y = Random.Range(areaCenter.y - half.y, areaCenter.y + half.y);
        float z = Random.Range(areaCenter.z - half.z, areaCenter.z + half.z);

        return new Vector3(x, y, z);
    }

    bool IsFarEnough(Vector3 candidate)
    {
        foreach (var t in aliveTargets)
        {
            if (t == null) continue;
            if (Vector3.Distance(candidate, t.transform.position) < minDistance)
                return false;
        }
        return true;
    }

    //Display the generation area in the Scene view (for convenient parameter adjustment)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
