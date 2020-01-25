using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontScale : MonoBehaviour
{
    public float UIBaseScreenHeight = 800f;
    public int BaseFontSize = 20;
    // public GameObject TextBox;

    private int GetScaledFontSize(int baseFontSize)
    {
        float uiScale = Screen.height / UIBaseScreenHeight;
        int scaledFontSize = Mathf.RoundToInt(baseFontSize * uiScale);
        return scaledFontSize;
    }

    void Start()
    {
        GetComponent<Text>().fontSize =GetScaledFontSize(BaseFontSize);
    }


}
