using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Deck : MonoBehaviour
{
    public CardObject cardPrefab;

    List<Card> cardsInDeck = new List<Card>();
    List<CardObject> cardsInHand = new List<CardObject>();

    List<CardObject> cardPool = new List<CardObject>();
    int cardPoolSize = 10;
    
    Cards allCards;
    
    int year = 0;

    // Start is called before the first frame update
    void Start()
    {
        allCards = new Cards();
        //AddCardToDeck(allCards.cards["Eat"]);
        AddCardToDeck(allCards.cards["Cry"]);
        //AddCardToDeck(allCards.cards["Eat"]);
        //AddCardToDeck(allCards.cards["Cry"]);
        //AddCardToDeck(allCards.cards["Eat"]);

        for (int i = 0; i < cardPoolSize; ++i) {
            CardObject cardObject = GameObject.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            cardObject.gameObject.SetActive(false);
            cardPool.Add(cardObject);
        }

        StartGame();
    }

    public Text deckCountText;
    void UpdateDeckSize() {
        deckCountText.text = cardsInDeck.Count.ToString();
    }

    void ReturnCardToPool(CardObject co) {
        co.hoverReady = false;
        co.StopHover();
        co.gameObject.SetActive(false);
        cardPool.Add(co);
    }

    CardObject GetCardFromPool() {
        CardObject cardObject;

        if (cardPool.Count > 0) {
            cardObject = cardPool[cardPool.Count - 1];
            cardPool.RemoveAt(cardPool.Count - 1);
        } else {
            cardObject = GameObject.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        }

        return cardObject;
    }
    
    int startHandSize = 4;


    void StartGame() {
        UpdateDeckSize();
        DealHand();
    }

    public float handPadding;

    void DealHand() {

        BoxCollider collider = cardPrefab.GetComponent<BoxCollider>();
        float cardWidth = collider.size.x;
        float halfCardWidth = cardWidth / 2.0f;
        float leftPos = handCenter.position.x - (halfCardWidth * startHandSize);

        float handOffset = 0;
        
        for (int i = 0; i < startHandSize; ++i) {
            DealCard(new Vector3(leftPos + cardWidth * i + halfCardWidth + handOffset, handCenter.position.y, handCenter.position.z));
            handOffset += handPadding;
        }
    }

    void AddCardToDeck(Card card) {
        Card newCard = new Card(card);
        cardsInDeck.Add(newCard);
    }

    // Deck does not have to really be "shuffled", a random card can just be drawn.
    Card GetCardFromDeck() {
        int index = Random.Range(0, cardsInDeck.Count);
        Card card = cardsInDeck[index];
        cardsInDeck.RemoveAt(index);
        UpdateDeckSize();
        return card;
    }

    public Transform handCenter;
    public Transform cardStartTransform;
    
    int cardsPlayedCount = 0;
    int cardsToPlayPerTurn = 2;

    void DealCard(Vector3 pos) {
        if (cardsInDeck.Count > 0) {
            Card card = GetCardFromDeck();

            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);
            newCard.InitializeAnimation(cardStartTransform.position, pos);
            newCard.gameObject.SetActive(true);

            cardsInHand.Add(newCard);
        }
    }

    void ActivateCard(CardObject cardObject) {

        foreach(Card c in cardObject.card.choiceCards) {
            if (!c.onetime || !allCards.CheckPlayed(c)) {
                AddCardToDeck(c);

                if (c.onetime) {
                    allCards.OneTime(cardObject.card);
                }
            }
        }
        
        cardsInHand.Remove(cardObject);
        ReturnCardToPool(cardObject);
        UpdateDeckSize();

        cardsPlayedCount++;

        if (cardsPlayedCount >= cardsToPlayPerTurn || cardsInHand.Count == 0) {
            NextTurn();
        }
    }   

    public TMPro.TextMeshProUGUI yearText;
    void UpdateYear() {
        year++;
        yearText.text = "Year " + year.ToString();
    }

    void NextTurn() {
        UpdateYear();

        cardsPlayedCount = 0;
        foreach (CardObject c in cardsInHand) {
            ReturnCardToPool(c);

            if (c.card.lifespan > 0) {
                c.card.lifespan--;
                AddCardToDeck(c.card);
            }
        }

        cardsInHand.Clear();

        DealHand();
    }

    CardObject hoveringOver;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //DealCard();
        }

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if ( Physics.Raycast(mouseRay, out info, 10.0f)) {
            CardObject cardObject = info.collider.GetComponent<CardObject>();

            if (hoveringOver != null && cardObject != hoveringOver) {
                hoveringOver.StopHover();
            }

            if(cardObject == null) {hoveringOver = null;}

            if (cardObject != null) {

                if (cardObject.StartHover()) {
                    hoveringOver = cardObject;
                }

                if (Input.GetMouseButtonDown(0)) {
                    ActivateCard(cardObject);
                    Debug.Log(cardObject.card.name);
                }
            }
        } else {
            if (hoveringOver != null) {
                hoveringOver.StopHover();
            }

            hoveringOver = null;
        }
    }

}
