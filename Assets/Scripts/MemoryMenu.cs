using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryMenu : MonoBehaviour
{
    public void OpenMenu()
    {

        SceneManager.LoadScene("MainMenu");
    }

}
