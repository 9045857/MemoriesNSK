using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Game;

    public GameObject GamePanel;
    public Sprite[] AllImages;
    public GameObject CardPrefab;

    private int _openedCardCount = 0;

    private CardManager _card1;
    private CardManager _card2;


    private Sprite[] _thisGameCardImages;
    private GameObject[] _cards;


    private float _offSet;

    private float _startX;
    private float _startY;



    private int _rowsCount;
    private int _columnCount;


    public int MaxCardCount = 4;
    public int CardShare = 10;
    public int SpacesShare = 10;

    private float _panelWidth;
    private float _panelHeight;
    private bool _isVerticalScreen;//маркер является ли экран вертикальным
    private float _cardSize;//длина стороны квадратной карточки
    private float _spaceBeforeActiveZone;//простатранство перед квадратной областью рисования карт
    private float _spaceBetweenCards;

    //  numbers по два одинаковых номера типа: { 0, 0, 1, 1, 2, 2, 3, 3 }
    private int[] numbers;

    public void OpenCard(CardManager card)
    {
        switch (_openedCardCount)
        {
            case 0:
                TurnCard(card);
                _card1 = card;
                break;

            case 1:
                TurnCard(card);
                _card2 = card;

                if (_card1.Id == _card2.Id)
                {
                    _card1.CloseCard();
                    _card2.CloseCard();

                    _openedCardCount = 0;
                }
                break;

            case 2:
                TurnCard(_card1);
                TurnCard(_card2);

                _openedCardCount = 0;

                TurnCard(card);
                _card1 = card;
                break;
        }
    }

    private void TurnCard(CardManager card)
    {
        if (!card.IsCardOpened)
        {
            _openedCardCount++;
            card.ShowCardFace();
            card.IsCardOpened = true;
        }
        else
        {
            card.ShowCardBack();
            card.IsCardOpened = false;
        }
    }

    private void SetBaseCardDrawParameters(GameObject parentPanel)
    {
        RectTransform rectTransform = parentPanel.GetComponent<RectTransform>();

        _panelWidth = rectTransform.rect.width;
        _panelHeight = rectTransform.rect.height;

        Debug.Log("ширина " + _panelWidth);
        Debug.Log("высота " + _panelHeight);

        float activeZoneSide;

        if (_panelHeight > _panelWidth)
        {
            _isVerticalScreen = true;
            _spaceBeforeActiveZone = (_panelHeight - _panelWidth) / 2;
            activeZoneSide = _panelWidth;
        }
        else
        {
            _isVerticalScreen = false;
            _spaceBeforeActiveZone = (_panelWidth - _panelHeight) / 2;
            activeZoneSide = _panelHeight;
        }

        _cardSize = activeZoneSide / (MaxCardCount + SpacesShare / CardShare);

        const int oneSpace = 1;
        _spaceBetweenCards = SpacesShare / CardShare * _cardSize / (MaxCardCount + oneSpace);
    }

    private void SetBeginNumbers()
    {
        int cardsCount = _rowsCount * _columnCount;
        numbers = new int[cardsCount];

        for (int i = 0; i < cardsCount / 2; i++)
        {
            numbers[i * 2] = i;
            numbers[i * 2 + 1] = i;
        }
    }

    private void SetColumnAndRowsCount()
    {
        _rowsCount = GameManagerScript.RowsCount;
        _columnCount = GameManagerScript.ColumnCount;
    }

    private void SetCardPositionSettings()
    {
        float xAddition = 0;
        float yAddition = 0;

        if (_isVerticalScreen)
        {
            yAddition = _spaceBeforeActiveZone;
        }
        else
        {
            xAddition = _spaceBeforeActiveZone;
        }

        float xSpaceIfColumnCountLessThenMaxCardCountHalf = (MaxCardCount - _columnCount) * _cardSize / 2;
        float leftPanelEdge = -_panelWidth / 2;
        float cardCenter = _cardSize / 2;

        _startX = leftPanelEdge + xAddition + _spaceBetweenCards + xSpaceIfColumnCountLessThenMaxCardCountHalf + cardCenter;

        float ySpaceIfColumnCountLessThenMaxCardCountHalf = (MaxCardCount - _rowsCount) * _cardSize / 2;
        float topPanelEdge = _panelHeight / 2;

        _startY = topPanelEdge - yAddition - _spaceBetweenCards - ySpaceIfColumnCountLessThenMaxCardCountHalf - cardCenter;

        _offSet = _cardSize + _spaceBetweenCards;
    }

    private void SetGame()
    {
        GameManager.Game = this;
    }

    private void SetGameImages()
    {
        int cardCount = _columnCount * _rowsCount / 2;
        _thisGameCardImages = new Sprite[cardCount];

        List<int> randomIndexes = new List<int>();

        for (int i = 0; i < _thisGameCardImages.Length; i++)
        {
            int index = Random.Range(0, AllImages.Length);

            while (randomIndexes.Contains(index))
            {
                index = Random.Range(0, AllImages.Length);
            }

            randomIndexes.Add(index);
            _thisGameCardImages[i] = AllImages[index];
        }
    }

    private void Start()
    {
        SetGame();//only for Start
        SetColumnAndRowsCount();//only for Start
        SetBaseCardDrawParameters(GamePanel);//only for Start

        SetGameImages();
        SetBeginNumbers();

        SetCardPositionSettings();//only for Start

        Shuffle();

        CreateCards();//only for Start
        DrawCards();
    }

    public void Restart()
    {
        SetGameImages();
        SetBeginNumbers();

        Shuffle();
        ReFillCards();
        DrawCards();

        _openedCardCount = 0;
    }

    private void ReFillCards()
    {
        for (int i = 0; i < _columnCount; i++)
        {
            for (int j = 0; j < _rowsCount; j++)
            {
                int index = GetCardArrayIndex(j, i);
                int imageId = numbers[index];

                _cards[index].GetComponent<CardManager>().SetImage(imageId, _thisGameCardImages[imageId]);
                _cards[index].GetComponent<CardManager>().DoReadyCard();
            }
        }
    }

    private void CreateCards()
    {
        _cards = new GameObject[_rowsCount * _columnCount];

        for (int i = 0; i < _columnCount; i++)
        {
            for (int j = 0; j < _rowsCount; j++)
            {
                GameObject newCard = Instantiate(CardPrefab);// as GameObject;
                newCard.transform.SetParent(GamePanel.transform, true);

                int index = GetCardArrayIndex(j, i);
                int imageId = numbers[index];

                newCard.GetComponent<CardManager>().SetImage(imageId, _thisGameCardImages[imageId]);

                _cards[index] = newCard;
            }
        }
    }

    private void DrawCards()
    {
        for (int i = 0; i < _columnCount; i++)
        {
            for (int j = 0; j < _rowsCount; j++)
            {
                int index = GetCardArrayIndex(j, i);
                GameObject card = _cards[index];

                float posX = _startX + i * _offSet;
                float posY = _startY - j * _offSet;

                Vector3 pos = new Vector3(posX, posY, 0);
                card.transform.localPosition = pos;

                card.transform.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cardSize);
                card.transform.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _cardSize);
                card.transform.localScale = new Vector3(1f, 1f, 0);
            }
        }
    }

    private int GetCardArrayIndex(int j, int i)
    {
        return j * _columnCount + i;
    }

    public void Shuffle()
    {
        int[] tmpArr = numbers.Clone() as int[];

        for (int i = 0; i < tmpArr.Length; i++)
        {
            int tmp = tmpArr[i];
            int rndpos = Random.Range(i, tmpArr.Length);
            tmpArr[i] = tmpArr[rndpos];
            tmpArr[rndpos] = tmp;
        }
        numbers = tmpArr;
    }

    void Update()
    {
        RectTransform rectTransform = GamePanel.GetComponent<RectTransform>();

        float panelWidth = rectTransform.rect.width;
        float panelHeight = rectTransform.rect.height;

        if (panelWidth != _panelWidth || panelHeight != _panelHeight)
        {
            SetBaseCardDrawParameters(GamePanel);
            SetCardPositionSettings();
            DrawCards();
        }
    }
}
