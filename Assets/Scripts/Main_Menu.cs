using UnityEngine;
using UnityEngine.SceneManagement;


public class Main_Menu : MonoBehaviour
{
    const double epsilon = 1.0e-10;

    public float HeightToWidthRatio = 2;

    public GameObject MenuPanel;

    private void Start()
    {
        SetPanelDrawParameters();
    }

    private void Update()
    {
        SetPanelDrawParameters();
    }


    private static bool AreEqual(float number1, float number2)
    {
        if (number1 >= number2)
        {
            return number1 - number2 < epsilon;
        }

        return number2 - number1 < epsilon;
    }

    private void SetPanelDrawParameters()
    {
        RectTransform rectTransform = MenuPanel.GetComponent<RectTransform>();

      //  float panelWidth = rectTransform.rect.width;
        float panelHeight = rectTransform.rect.height;

        if (!AreEqual(panelHeight, Screen.height))
        {
            float newHeight=Screen.height;
            float newWidth = newHeight / HeightToWidthRatio;

            MenuPanel.transform.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            MenuPanel.transform.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            float posX = 0;
            float posY = 0;

            Vector3 pos = new Vector3(posX, posY, 0);
            MenuPanel.transform.localPosition = pos;

        }
    }



    public void OpenMemory43()
    {
        GameManagerScript.ColumnCount = 4;
        GameManagerScript.RowsCount = 3;

        SceneManager.LoadScene("Memore");
    }

    public void OpenMemory44()
    {
        GameManagerScript.ColumnCount = 4;
        GameManagerScript.RowsCount = 4;

        SceneManager.LoadScene("Memore");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenIinstruction()
    {
        SceneManager.LoadScene("Iinstruction");
    }

    public void OpenEyePreparation()
    {
        SceneManager.LoadScene("Eye");
    }

    public void OpenMemory42()
    {
        GameManagerScript.ColumnCount = 4;
        GameManagerScript.RowsCount = 2;

        SceneManager.LoadScene("Memore");
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
