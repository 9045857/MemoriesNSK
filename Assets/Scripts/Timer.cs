using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float StartTime;
    public float EndTime;

    void FixedUpdate()
    {
        StartTime += 0.1f;

        if (StartTime >= EndTime)
        {
           // Destroy(gameObject);
        }

    }
}
