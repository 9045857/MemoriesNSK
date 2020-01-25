using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void OpenMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
