using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Training : MonoBehaviour
{
    public float Angle;

    public float StartTime;
    public float EndTime;

    // Start is called before the first frame update
    void Start()
    {
        //StartTime = 0;
        //EndTime = 49;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (StartTime <= EndTime)
        {
            Quaternion rotationZ = Quaternion.AngleAxis(Angle, Vector3.forward);
            transform.rotation *= rotationZ;
            StartTime += 0.1f;
        }
        else
        {
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
    }
}
