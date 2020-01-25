using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    private static GameManagerScript ManagerScript;

    public static int RowsCount;
    public static int ColumnCount;

    private void Start()
    {
        if (ManagerScript != null)
        {
            UnityEngine.GameObject.Destroy(gameObject);
        }
        else
        {
            UnityEngine.GameObject.DontDestroyOnLoad(gameObject);
            ManagerScript = this;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        //Text ui = GameObject.Find("/Canvas/Text").GetComponent<Text>();
        Debug.Log(level + ":  " + RowsCount + "  " + ColumnCount);
    }
}


