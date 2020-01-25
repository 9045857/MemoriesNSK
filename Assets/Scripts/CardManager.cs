using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject Card;
    public int Id;

    private GameManager game;

    public Sprite CardFaceImage;
    public Sprite CardBackImage;

    public bool IsCardInGame { get; set; }
    public bool IsCardOpened { get; set; }

    void Start()
    {
        game = GameManager.Game;
        IsCardInGame = true;
        IsCardOpened = false;
    }

    public void OnMouseClick()
    {
        if (IsCardInGame && !IsCardOpened && Card.activeSelf)
        {
            game.OpenCard(this);
        }
    }

    public void ShowCardFace()
    {
        Card.transform.Find("Cap").GetComponent<Image>().sprite = CardFaceImage;
    }

    public void ShowCardBack()
    {
        Card.transform.Find("Cap").GetComponent<Image>().sprite = CardBackImage;
    }

    public void CloseCard()
    {
        IsCardInGame = false;
        Card.GetComponent<LeanButton>().enabled = false;
    }

    public void DoReadyCard()
    {
        ShowCardBack();
        IsCardInGame = true;
        IsCardOpened = false;
        Card.GetComponent<LeanButton>().enabled = true;
    }

    public void SetImage(int id, Sprite faceCardImage)
    {
        Id = id;
        CardFaceImage = faceCardImage;
    }
}
