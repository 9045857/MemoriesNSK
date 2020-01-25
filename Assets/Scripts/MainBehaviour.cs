using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

public class MainBehaviour : MonoBehaviour
{
    public MemGameSettings Settings;

    public UnityEngine.GameObject CardTemplate;
    public UnityEngine.GameObject ColContainer;
    public UnityEngine.GameObject CardContainerTemplate;

    public Sprite[] CardFaces;

    private CardScript[] cards;
    public int CardsPressed = 0;
    public int FirstCardIndex = -1;

    public int CloseCardsCounter;

    void Awake()
    {
        Settings.ColCount = GameManagerScript.ColumnCount;
        Settings.RowCount = GameManagerScript.RowsCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        PutCards();
    }

    public void RestartGame()
    {
        ClearGame();
        PutCards();
    }

    private void ClearGame()
    {

    }

    private void PutCards()
    {
        CloseCardsCounter = 0;
        // Создаем для ColContainer нужное количесвто кард-контейнеров (вертикальные стопки карт)

        int CardCount = Settings.RowCount * Settings.ColCount;
        int[] indexes = new int[CardCount];

        cards = new CardScript[CardCount];

        for (int i = 0; i < CardCount / 2; i++)
        {
            indexes[i * 2] = i;
            indexes[i * 2 + 1] = i;
        }

        indexes = GetShuffledIndexes(indexes);

        for (int i = 0; i < Settings.ColCount; i++)
        {
            UnityEngine.GameObject copy = Instantiate(CardContainerTemplate);
            copy.transform.SetParent(ColContainer.transform, false);
            for (int j = 0; j < Settings.RowCount; j++)
            {
                UnityEngine.GameObject copyCard = Instantiate(CardTemplate);
                copyCard.transform.SetParent(copy.transform, false);

                CardScript card = (CardScript)copyCard.GetComponent(typeof(CardScript));

                int idx = i * Settings.RowCount + j;

                card.Front = CardFaces[indexes[idx]];
                card.IsOpened = false;
                card.IsSolved = false;
                card.ImageIndex = indexes[idx];

                cards[idx] = card;
                copyCard.GetComponent<LeanButton>().OnClick.AddListener(delegate { CardPressed(idx); });

                Debug.Log("Click " + idx.ToString());
            }
        }
    }

    private int[] GetShuffledIndexes(int[] indexes)
    {
        int[] tmpArr = indexes.Clone() as int[];

        for (int i = 0; i < tmpArr.Length; i++)
        {
            int tmp = tmpArr[i];
            int rndpos = Random.Range(i, tmpArr.Length);
            tmpArr[i] = tmpArr[rndpos];
            tmpArr[rndpos] = tmp;
        }
        return tmpArr;
    }

    public void CardPressed(int index)
    {
        switch (CardsPressed)
        {
            case 0:
                {
                    // открываем первую карту
                    FirstCardIndex = index;
                    CardsPressed++;
                    break;
                }
            case 1:
                {
                    // открываем вторую карту
                    CardsPressed++;

                    if (cards[FirstCardIndex].ImageIndex == cards[index].ImageIndex)
                    {
                        Debug.Log("Found");
                        cards[FirstCardIndex].SetSolved();
                        cards[index].SetSolved();
                    }
                    else
                    {
                        CloseCardsCounter = 250;
                    }
                    break;
                }

            case 2:
                {
                    // две карты уже открыты и жмем третью.

                    CloseCards();

                    CardsPressed = 1;
                    cards[index].ChangeCard();
                    FirstCardIndex = index;
                    break;
                }
            default:
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (CloseCardsCounter > 0)
        {
            CloseCardsCounter--;
            if (CloseCardsCounter == 0)
                CloseCards();
        }
    }

    public void CloseCards()
    {
        foreach (CardScript card in cards)
        {
            if (card.IsOpened && !card.IsSolved)
                card.ChangeCard();
        }
        CardsPressed = 0;
        CloseCardsCounter = 0;
    }


}

[System.Serializable]
public class MemGameSettings
{
    public int ColCount;
    public int RowCount;
}
