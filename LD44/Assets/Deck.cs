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

    List<CardObject> previewCards = new List<CardObject>();

    int cardPoolSize = 40;
    
    Cards allCards;
    
    int year = 0;

    // Start is called before the first frame update
    void Start()
    {
        allCards = new Cards();
        AddCardToDeck(allCards.cards["Eat"]);
        AddCardToDeck(allCards.cards["Cry"]);
        AddCardToDeck(allCards.cards["Smelly Socks"]);

        AddCardToDeck(allCards.cards["Eat"]);
        //AddCardToDeck(allCards.cards["Cry"]);
        //AddCardToDeck(allCards.cards["Eat"]);

        for (int i = 0; i < cardPoolSize; ++i) {
            CardObject cardObject = GameObject.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            cardObject.transform.position = new Vector3(1000, 1000, 0);

           // cardObject.gameObject.SetActive(false);
            cardPool.Add(cardObject);
        }

        StartGame();
    }

    public Text deckCountText;
    void UpdateDeckSize() {
        deckCountText.text = cardsInDeck.Count.ToString();
    }

    public void ReturnCardToPool(CardObject co) {
        co.hoverReady = false;
        co.StopHover();
        co.enabled = false;
        co.transform.position = new Vector3(1000, 1000, 0);
       // co.gameObject.SetActive(false);
        cardPool.Add(co);
     //   Debug.Log("returned card to pool");
    }

    CardObject GetCardFromPool() {
        CardObject cardObject;

        if (cardPool.Count > 0) {
            cardObject = cardPool[cardPool.Count - 1];
            cardPool.RemoveAt(cardPool.Count - 1);
        } else {
            cardObject = GameObject.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        }

        cardObject.deck = this;
        cardObject.enabled = true;
        //cardObject.gameObject.SetActive(true);
        return cardObject;
    }
    
    int startHandSize = 4;

    void StartGame() {
        UpdateDeckSize();
        DealHand();
    }

    public int handSizeAdjustment = 3;
    public int handSizeAdjustmentHowManyTurns = 0;

    public float handPadding;

    public Transform previewCardPos;

    public float choicePadding = .1f;


    void CreatePreviewCard(CardObject newCard, ref float currentX, float cardWidth) {
        newCard.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        newCard.isPreview = true;
        newCard.baseScale = .8f;
        newCard.gameObject.transform.position = previewCardPos.position + Vector3.right * currentX;
        newCard.timer = newCard.timerMax;
        newCard.hoverTimer = newCard.hoverTimerMax;

        newCard.targetPos = newCard.gameObject.transform.position;

        currentX += cardWidth + choicePadding;

        previewCards.Add(newCard);
    }
    void GeneratePreviewCards(CardObject co) {
       // Debug.Log("Generating Preview Cards");
        HidePreviewCards();

        float currentX = 0;

        BoxCollider collider = cardPrefab.GetComponent<BoxCollider>();
        float cardWidth = collider.size.x * .8f;

        foreach(Card card in co.card.choiceCards) {
            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);
            CreatePreviewCard(newCard, ref currentX, cardWidth);
        }

        currentX += choicePadding;
        
        foreach(Card card in co.card.tragicEvents) {
            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);
            CreatePreviewCard(newCard, ref currentX, cardWidth);
        }
    }

    void HidePreviewCards() {

        foreach(CardObject c in previewCards) {
            c.StartFadeOut(0);
            c.returnWhenAnimationDone = true;
            //ReturnCardToPool(c);
        }

        previewCards.Clear();
    }
    
    public float animationOffset = .1f;

    void DealHand() {

        BoxCollider collider = cardPrefab.GetComponent<BoxCollider>();
        float cardWidth = collider.size.x;
        float halfCardWidth = cardWidth / 2.0f;
        
        float leftPos = handCenter.position.x;

        float handOffset = 0;
        
        int cardsToBeDealt = Mathf.Min(startHandSize, cardsInDeck.Count);
        
        if (handSizeAdjustmentHowManyTurns > 0) {
            cardsToBeDealt = handSizeAdjustment;
            handSizeAdjustmentHowManyTurns--;
        }

        for (int i = 0; i < cardsToBeDealt; ++i) {
            DealCard(
                new Vector3(leftPos + cardWidth * i + handOffset, handCenter.position.y, handCenter.position.z),
                i, 
                cardsToBeDealt
            );
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

    void DealCard(Vector3 pos, int index, int totalCount) {

        float animationTotal = totalCount * animationOffset + .4f;
        
        if (cardsInDeck.Count > 0) {
            Card card = GetCardFromDeck();

            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);

            newCard.InitializeAnimation(-index * animationOffset, cardStartTransform.position, pos);
            //newCard.gameObject.SetActive(true);

            if (card.type == CardType.Choice) {
                newCard.StartRotation(-animationTotal + -index * animationOffset, 180, 0);
            } else if (card.type == CardType.TragicEvent) {
                newCard.StartRotation(-animationTotal * 1.6f + -index * animationOffset, 180, 0);
                //newCard.transform.rotation = Quaternion.Euler(0, 180, 0);
                //newCard.StartHover();
            }
            
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

        foreach(Card c in cardObject.card.tragicEvents) {
            if (!c.onetime || !allCards.CheckPlayed(c)) {
                AddCardToDeck(c);

                if (c.onetime) {
                    allCards.OneTime(cardObject.card);
                }
            }
        }

        if (cardObject.card.handSizeAdjustmentTurns > 0) {
            handSizeAdjustment = cardObject.card.handSizeAdjustment;
            handSizeAdjustmentHowManyTurns = cardObject.card.handSizeAdjustmentTurns;
        }

        float timerOffset = 0;
        float moveOffset = previewCards.Count * animationOffset + .5f;

        foreach(CardObject c in previewCards) {
            if (c.card.type == CardType.Choice) {
                c.StartRotation(timerOffset, 180, 0);
            }

            c.returnWhenAnimationDone = true;
            c.InitializeAnimation(-moveOffset + timerOffset, c.gameObject.transform.position, cardStartTransform.position);
            timerOffset -= animationOffset;
        }
        
        previewCards.Clear();

        cardsInHand.Remove(cardObject);
        ReturnCardToPool(cardObject);
        UpdateDeckSize();

        cardsPlayedCount++;
        CheckEndState();

    }   

    public TMPro.TextMeshProUGUI yearText;
    void UpdateYear() {
        year++;
        yearText.text = "Year " + year.ToString();
    }

    void ShuffleBackCards() {
        List<CardObject> toRemove = new List<CardObject>();

       foreach (CardObject c in cardsInHand) {
            if (c.card.type == CardType.Choice) {
                //ReturnCardToPool(c);

                if (c.card.lifespan > 0) {
                    c.card.lifespan--;
                    AddCardToDeck(c.card);

                    c.InitializeAnimation(0, c.transform.position, cardStartTransform.position);
                    c.returnWhenAnimationDone = true;
                } else { 
                   DiscardAnimation(c);
                }

                toRemove.Add(c);
            }
        }

        foreach(CardObject co in toRemove) {
            cardsInHand.Remove(co);
        }

    }

    void NextTurn() {
        UpdateYear();

        cardsPlayedCount = 0;
        ShuffleBackCards();

        previewCards.Clear();
        cardsInHand.Clear();

        DealHand();
    }
    
    void DiscardAnimation(CardObject c) {
        c.InitializeAnimation(0, c.transform.position, c.transform.position + Vector3.down * 4.0f);
        c.returnWhenAnimationDone = true;
    }

    void DiscardHandIndex(int i) {
        Vector3 pos = cardsInHand[i].transform.position;
        DiscardAnimation(cardsInHand[i]);
        cardsInHand.Remove(cardsInHand[i]);

        DealCard(pos, 2, 0);

        CheckEndState();
    }

    void CheckEndState() {
        int eventCardCount = 0;
        foreach(CardObject card in cardsInHand) {
            if (card.card.type == CardType.TragicEvent) {
                eventCardCount++;
            }
        }
        if ((cardsPlayedCount >= cardsToPlayPerTurn || cardsInHand.Count == 0)) {
            if (eventCardCount == 0) {
                NextTurn();
            } else {
                ShuffleBackCards();
            }
        }
    }


    CardObject hoveringOver;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            DiscardHandIndex(2);
        }

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if ( Physics.Raycast(mouseRay, out info, 10.0f)) {
            CardObject cardObject = info.collider.GetComponent<CardObject>();

            if (cardObject != null && cardObject.isPreview) {
                cardObject = null;
            }
            
            if (hoveringOver != null && cardObject != hoveringOver) {
                hoveringOver.StopHover();
                HidePreviewCards();
            }

            if(cardObject == null) {hoveringOver = null;}

            if (cardObject != null) {

                if (cardObject.StartHover()) {
                    hoveringOver = cardObject;
                    GeneratePreviewCards(cardObject);
                }

                if (Input.GetMouseButtonDown(0)) {
                    ActivateCard(cardObject);
                   // Debug.Log(cardObject.card.name);
                }
            }
        } else {
            if (hoveringOver != null) {
                hoveringOver.StopHover();
                HidePreviewCards();
            }

            hoveringOver = null;
        }
    }

    void TriggerTragicEvent(CardObject card) {
       // card.StartRotation()
    }

}
