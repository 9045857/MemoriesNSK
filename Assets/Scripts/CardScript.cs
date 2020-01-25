using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    public UnityEngine.GameObject CardButton;
    public int Width;
    public int Height;

    public bool IsOpened;
    public bool IsSolved;
    public int ImageIndex;

    public Sprite Back;
    public Sprite Front;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSolved()
    {
        CardButton.SetActive(false);
        IsSolved = true;
    }

    public void ChangeCard()
    {
        if (!IsSolved)
        {
            IsOpened = !IsOpened;
            CardButton.GetComponent<Image>().sprite = IsOpened ? Front : Back;
        }
    }
}
