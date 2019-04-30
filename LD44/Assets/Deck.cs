using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Deck : MonoBehaviour
{
    public List<string> turnStrings = new List<string>();
    
    public CardObject cardPrefab;

    List<Card> cardsInDeck = new List<Card>();
    List<CardObject> cardsInHand = new List<CardObject>();

    List<CardObject> cardPool = new List<CardObject>();

    List<CardObject> previewCards = new List<CardObject>();
    
    List<bool> openHandIndices = new List<bool>();

    int cardPoolSize = 40;
    
    Cards allCards;
    
    int year = 0;

    public int animationsWaitingOn = 0;

    public int visibleDeckSize = 0;
    
    float baseCardWidth; 
    // Start is called before the first frame update
    
    void Start()
    {   
        Card.randomEnabled = false;
        for (int i = 0; i < maxHandSize; ++i) {
            openHandIndices.Add(true);
        }

        baseCardWidth = cardPrefab.GetComponent<BoxCollider>().size.x;

        targetBackgroundColor = backgroundColor;
        allCards = new Cards();
        AddCardToDeck(allCards.cards["Begin your life"]);

        for (int i = 0; i < cardPoolSize; ++i) {
            CardObject cardObject = GameObject.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            cardObject.transform.position = new Vector3(1000, 1000, 0);

           // cardObject.gameObject.SetActive(false);
            cardPool.Add(cardObject);
        }

        visibleDeckSize = cardsInDeck.Count;

        StartGame();

        UpdateDeckSize();
    }

    public Text deckCountText;
    public void UpdateDeckSize() {
        int num = visibleDeckSize;
        if(visibleDeckSize < 0) {
            num = 0;
        }
        string text = num.ToString() + " card";
        if (num != 1) {
            text += "s";
        }

        deckCountText.text = text;
    }

    public void ReturnCardToPool(CardObject co) {
        co.hoverReady = false;
        co.StopHover();
        co.enabled = false;
        co.transform.position = new Vector3(1000, 1000, 0);
       // co.gameObject.SetActive(false);
        cardPool.Add(co);

        if (co.waitingForAnimation) {
            animationsWaitingOn--;
        }

        if (co.incrementDeckCountWhenDone) {
            //Debug.Log("+1 to Deck Size: " + co.name);
            visibleDeckSize++;
            UpdateDeckSize();
        }

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
    int maxHandSize = 5;
    
    int randomCards = 5;
    void StartGame() {
        UpdateDeckSize();
        DealHand();
        UpdateYear();

    }

    public int handSizeAdjustment = 3;
    public int handSizeAdjustmentHowManyTurns = 0;

    public float handPadding;

    public Transform previewCardPos;

    public float choicePadding = .1f;


    void CreatePreviewCard(CardObject newCard, ref float currentX, float cardWidth, float angle) {
        newCard.gameObject.transform.rotation = Quaternion.Euler(0, angle, 0);

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

        float cardWidth = baseCardWidth * .8f;

  
        foreach(Card card in co.card.choiceCards) {
            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);

            float angle = 0;
            if (card.mystery) angle = 180;

            CreatePreviewCard(newCard, ref currentX, cardWidth, angle);
        }

        currentX += choicePadding;
        
        foreach(Card card in co.card.tragicEvents) {
            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);

            float angle = 0;
            if (card.mystery) angle = 180;

            CreatePreviewCard(newCard, ref currentX, cardWidth, angle);
        }

        currentX += choicePadding;

        foreach(Card card in co.card.events) {
            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);

            float angle = 0;
            if (card.mystery) angle = 180;

            CreatePreviewCard(newCard, ref currentX, cardWidth, angle);
        }
        currentX += choicePadding;

        
        foreach(Card card in co.card.randomEvents) {
            CardObject newCard = GetCardFromPool();
            newCard.InitializeCard(card);

            float angle = 0;
            if (card.mystery) angle = 180;

            CreatePreviewCard(newCard, ref currentX, cardWidth, angle);
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

    int firstDealCounter = 3;

    Vector3 CalculatePosition(int index) {
        BoxCollider collider = cardPrefab.GetComponent<BoxCollider>();
        Vector3 pos = new Vector3(handCenter.position.x + baseCardWidth * index + index * handPadding, handCenter.position.y, handCenter.position.z);
        return pos;
    }

    void DealHand() {     
        handsDealt++;

        firstDealCounter--;

        if ((cardsInDeck.Count <= startHandSize && animationsWaitingOn > 0) || firstDealCounter > 0) {
            delayedDealHand = true;
            return;
        }

        BoxCollider collider = cardPrefab.GetComponent<BoxCollider>();
                
        int cardsToBeDealt = Mathf.Min(startHandSize, cardsInDeck.Count);
        
        if (handSizeAdjustmentHowManyTurns > 0) {
            cardsToBeDealt = handSizeAdjustment;
            handSizeAdjustmentHowManyTurns--;
        }

        for (int i = 0; i < cardsToBeDealt; ++i) {
            DealCard( 
                cardsToBeDealt, 
                i
            );
        }

        GenerateTopDeck();

        cardsPlayedCount = 0;
        
       // AddCardToDeck(allCards.cards["Smelly Socks"]);
      //  visibleDeckSize++;
      //  GenerateTopDeck();
       // UpdateDeckSize();
    }

    CardObject topCard;
    int topCardIndex;

    int AddCardToDeck(Card card) {
        Card copiedCard = new Card(card);
        cardsInDeck.Add(copiedCard);
        
        GenerateTopDeck();
        return cardsInDeck.Count - 1;
    }

    bool topCardReady = false;

    void SetTopDeck(int index) {
        CardObject newCard = GetCardFromPool();
        Card c = cardsInDeck[index];
        newCard.InitializeCard(c);
        
        newCard.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        newCard.isPreview = true;
        newCard.baseScale = .8f;
        newCard.gameObject.transform.position = cardStartTransform.position;
        newCard.targetPos = cardStartTransform.position;

        newCard.timer = newCard.timerMax;
        newCard.hoverTimer = newCard.hoverTimerMax;
        topCard = newCard;
        topCardIndex = index;
        topCardReady = true;
    }

    void GenerateTopDeck() {

        if (topCard != null) {
           // topCard.returnWhenAnimationDone = true;
          //  topCard.StartFadeOut(0);
            topCardReady = false;
        }

        if (cardsInDeck.Count == 0) return;

        // Next card
        int randomIndex = Random.Range(0, cardsInDeck.Count);
        SetTopDeck(randomIndex);
    }
    
    Card forceDrawCard = null;

    // Deck does not have to really be "shuffled", a random card can just be drawn.
    Card GetCardFromDeck() {
        int index = Random.Range(0, cardsInDeck.Count);
        
        if (forceDrawCard != null) {
            Card card = forceDrawCard;
            cardsInDeck.Remove(card);
            UpdateDeckSize();
            topCardReady = false;
            forceDrawCard = null;
            return card;
        }

        if (topCard != null && topCardReady) {
            Card card = topCard.card;
            cardsInDeck.Remove(card);
            topCardReady = false;
            UpdateDeckSize();
            return card;
        } else {
            Card card = cardsInDeck[index];
            cardsInDeck.RemoveAt(index);
            UpdateDeckSize();
            return card;
        }
    }

    public Transform handCenter;
    public Transform cardStartTransform;
    
    int cardsPlayedCount = 0;
    int cardsToPlayPerTurn = 2;

    void DealCard(int totalCount, int animationIndex) {
        if (cardsInHand.Count >= maxHandSize) {
            return;
        }

        int index = GetOpenHandIndex();

        if (index == -1) return;

        Vector3 pos = CalculatePosition(index);

        float animationTotal = totalCount * animationOffset + .4f;
        
        if (cardsInDeck.Count > 0) {
            openHandIndices[index] = false;
            
            Card card = GetCardFromDeck();

            CardObject newCard = GetCardFromPool();
            newCard.handIndex = index;
            newCard.InitializeCard(card);

            newCard.decrementDeckCountWhenStart = true;
            newCard.InitializeAnimation(-animationIndex * animationOffset, cardStartTransform.position, pos);

            //newCard.gameObject.SetActive(true);
            
            /*
            if (card.lifespan > 0 && firstNumberedCard) {
                firstNumberedCard = false;
                tipText.text = "Unused cards with numbers in the corner will shuffle back into your life";
            }
            */

            if (card.type == CardType.Choice) {
                newCard.StartRotation(-animationTotal + -animationIndex * animationOffset, 180, 0);
            } else if (card.type == CardType.Event) {
                newCard.StartRotation(-animationTotal * 1.3f + -animationIndex * animationOffset - .1f, 180, 0);
            } else if (card.type == CardType.TragicEvent) {

                newCard.StartRotation(-animationTotal * 1.6f + -animationIndex * animationOffset - .4f, 180, 0);
            }
            
            cardsInHand.Add(newCard);
        }
    }
    
    int GetOpenHandIndex() {
        for (int i = 0; i < maxHandSize; ++i) {
            if (openHandIndices[i]) {
                openHandIndices[i] = false;
                return i;
            }
        }
        return -1;
    }

    int yearCounter = 0;

    int tipCounter = 0;
    int yearIncrementRate = 3;

    int starCount = 0;

    public TMPro.TextMeshProUGUI starText;

    void ActivateCard(CardObject cardObject) {

        openHandIndices[cardObject.handIndex] = true;

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

        foreach(Card c in cardObject.card.events) {
            if (!c.onetime || !allCards.CheckPlayed(c)) {
                AddCardToDeck(c);

                if (c.onetime) {
                    allCards.OneTime(cardObject.card);
                }
            }
        }

        foreach(Card c in cardObject.card.randomEvents) {
            Card random = c.GetRandomOutcome();
            random.randomCardback = true;
            AddCardToDeck(random);
        }

        if (cardObject.card.handSizeAdjustmentTurns > 0) {
            handSizeAdjustment = cardObject.card.handSizeAdjustment;
            handSizeAdjustmentHowManyTurns = cardObject.card.handSizeAdjustmentTurns;
        }

        float timerOffset = 0;
        float moveOffset = previewCards.Count * animationOffset + .5f;

        foreach(CardObject c in previewCards) {

            /*
            if (c.card.type == CardType.Choice && !c.card.mystery) {
                c.StartRotation(timerOffset, 180, 0);
            }*/

            c.returnWhenAnimationDone = true;
            c.incrementDeckCountWhenDone = true;
            c.InitializeAnimation(-moveOffset + timerOffset, c.gameObject.transform.position, cardStartTransform.position);
            animationsWaitingOn++;
            c.waitingForAnimation = true;

            timerOffset -= animationOffset;
        }
        
        previewCards.Clear();

        cardsInHand.Remove(cardObject);
        openHandIndices[cardObject.handIndex] = true;
            
        if(cardObject.card.type == CardType.Choice) {
            cardsPlayedCount++;
        }

        if(cardObject.card.cardDiscard > 0) {
            DiscardRandomCards(cardObject.card.cardDiscard);
        }

        if(cardObject.card.cardDraw > 0) {
            for (int i = 0; i < cardObject.card.cardDraw; ++i) {
                DrawCard();
            }
        }

        if(cardObject.card.timeMachine) {
            year = 0;
        }

        if(cardObject.card.gainStar) {
            starCount++;
            starText.text = starCount.ToString();
        }

        if(cardObject.card.loseStar) {
            starCount--;
            starText.text = starCount.ToString();
        }



        UpdateDeckSize();    

        CheckEndState();
        FlashColor(cardObject.GetPrimaryColor(), .3f);

        yearCounter++;
        tipCounter++;

        if (yearCounter >= yearIncrementRate) {
            year++;
            UpdateYear();
            yearCounter = 0;
        }

        ReturnCardToPool(cardObject);
    }   

    public TMPro.TextMeshProUGUI yearText;
    public TMPro.TextMeshProUGUI tipText;
    public TMPro.TextMeshProUGUI title;
    public Rigidbody titleCard;

    void ShuffleInRandom(int count) {
        for (int i = 0; i < count; ++i) {
            AddCardToDeck(Cards.randomCards[Random.Range(0, Cards.randomCards.Count)]);
        }
    }

    int lastYear = 0;

    void ForceTopDeck(string cardName) {
        int index = AddCardToDeck(allCards.cards[cardName]);
        SetTopDeck(index);
        forceDrawCard = cardsInDeck[index];
    }
    
    int handsDealt = -1;

    void UpdateYear() {

        int tip = tipCounter;
        Debug.Log("TIP " + tip);

        if (turnStrings.Count > tip) {
            tipText.SetText(turnStrings[tip]);
        } else {
            tipText.SetText("");
        }
       // year++;
        yearText.text = "Year " + (year+1).ToString();

        if (lastYear == 3 && year == 4) {
            yearIncrementRate = 2;
            ForceTopDeck("First day of school");
        }
        
        if (year == 12 && lastYear == 11) {
            yearIncrementRate = 4;
            ForceTopDeck("Puberty");
        }
                
        if (year == 17 && lastYear == 16) {
            ForceTopDeck("Adulthood");
            yearIncrementRate = 2;
        }

        if (year == 39 && lastYear == 38) {
            ForceTopDeck("Midlife");
            yearIncrementRate = 1;
        }

        if (year > 30) {
            ForceTopDeck("A strange machine makes you a baby again");
        }

        lastYear = year;
        
    }
    

    bool firstEvent = true;

    bool firstNumberedCard = true;
    void ShuffleBackCards() {
        List<CardObject> toRemove = new List<CardObject>();

       foreach (CardObject c in cardsInHand) {
            if (c.card.type == CardType.Choice) {
                //ReturnCardToPool(c);

                if (c.card.lifespan > 0) {
                    c.card.lifespan--;
                    AddCardToDeck(c.card);

                    c.InitializeAnimation(0, c.transform.position, cardStartTransform.position);
                    c.incrementDeckCountWhenDone = true;
                    c.returnWhenAnimationDone = true;
                    animationsWaitingOn++;
                    c.waitingForAnimation = true;

                } else { 
                   DiscardAnimation(c);
                }

                toRemove.Add(c);
            }
        }

        foreach(CardObject co in toRemove) {
            cardsInHand.Remove(co);
            openHandIndices[co.handIndex] = true;
        }

    }

    public Color gameBackgroundColor;
    bool delayedDealHand = false;

    void NextTurn() {
       // titleCard.isKinematic = false;
        title.gameObject.SetActive(false);
        backgroundColor = gameBackgroundColor;

        UpdateYear();

        cardsPlayedCount = 0;
        ShuffleBackCards();

        previewCards.Clear();
        cardsInHand.Clear();

        DealHand();
    }
    
    void DiscardAnimation(CardObject c) {
        c.InitializeAnimation(0, c.transform.position, c.transform.position + Vector3.down * 8.0f);
        c.returnWhenAnimationDone = true;
        animationsWaitingOn++;
        c.waitingForAnimation = true;
    }

    void DiscardHandIndex(int i) {
        DiscardAnimation(cardsInHand[i]);

        cardsInHand.Remove(cardsInHand[i]);
        openHandIndices[i] = true;

        DealCard(0, 0);

        CheckEndState();
        GenerateTopDeck();
    }

    void DiscardCard(CardObject c) {
        DiscardAnimation(c);
        cardsInHand.Remove(c);
        openHandIndices[c.handIndex] = true;
    }

    void DiscardRandomCards(int count) {
        for (int i = 0; i < count && cardsInHand.Count > 0; ++i) {
            CardObject randomCard = cardsInHand[Random.Range(0, cardsInHand.Count)];
            DiscardCard(randomCard);
        }
    }

    bool victory = false;

    int starsRequired = 6;

    void CheckEndState() {
        int eventCardCount = 0;
        foreach(CardObject card in cardsInHand) {
            if (card.card.type == CardType.TragicEvent || card.card.type == CardType.Event) {
                eventCardCount++;
            }
        }

        if (cardsInHand.Count == 0) {
            NextTurn();
        }

        
        if (starCount >= starsRequired) {
            victory = true;
            DiscardRandomCards(8);
            title.text = "You win life!";
            title.gameObject.SetActive(true);
            tipText.text = "Click to play again";
        }
        
        /*
        if ((cardsPlayedCount >= cardsToPlayPerTurn || cardsInHand.Count == 0)) {
            if (eventCardCount == 0) {
                NextTurn();
            } else {
                //ShuffleBackCards();
            }
        }*/
    }


    CardObject hoveringOver;

    public AnimationCurve cameraColorPulse;

    public Color backgroundColor;

    public Color targetBackgroundColor;
    
    public float backgroundFadeSpeed = .1f;

    void SetBackGroundColorTarget(Color color) {
        targetBackgroundColor = color;
    }

    void FlashColor(Color color, float duration) {
        tempColor = color; 
        tempTimer = duration;
    }
    
    Color tempColor;
    float tempTimer;
    void MainCameraFlashFade() {
        if (tempTimer > 0) {
            targetBackgroundColor = tempColor;
            tempTimer -= Time.deltaTime;
        } else {
            targetBackgroundColor = backgroundColor;
        }

        Camera.main.backgroundColor = Color.Lerp( Camera.main.backgroundColor , targetBackgroundColor, Time.deltaTime / backgroundFadeSpeed);
    }
    
    void DrawCard() {
        if (openHandIndices.Count > 0 && cardsInDeck.Count > 0) {
            DealCard(
                0,
                0
            );
        }
    }

    bool gameOver = false;

    public Material background;
    public Vector2 backgroundOffset;

    public Color backgroundVictoryColor;

    public TMPro.TextMeshProUGUI quoteText;

    void Update() {
        MainCameraFlashFade();

        if (victory) {
            if (Input.GetMouseButtonDown(0)) {
                Application.LoadLevel(Application.loadedLevel);
            }
            FlashColor(backgroundVictoryColor, .2f);
            quoteText.enabled = false;
            backgroundOffset += Time.deltaTime * new Vector2(.7f, -.9f).normalized;
            background.mainTextureOffset = backgroundOffset;

            return;
        }

        if (cardsInDeck.Count == 0 && cardsInHand.Count == 0) {
            tipText.text = "Your life has run out of cards. :( Click to try again";
            if (Input.GetMouseButtonDown(0)) {
                Application.LoadLevel(Application.loadedLevel);
            }
        }


        if (delayedDealHand && animationsWaitingOn <= 1) {
            delayedDealHand = false;
            animationsWaitingOn = 0;
            DealHand();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            //DiscardHandIndex(2);
           // DrawCard();
        }

        
        if (Input.GetKeyDown(KeyCode.D)) {
          //  DiscardHandIndex(2);
           // DiscardRandomCards(2);
           // DrawCard();
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
                SetBackGroundColorTarget(backgroundColor);

                HidePreviewCards();
            }

            if(cardObject == null) {hoveringOver = null;}

            if (cardObject != null ) {
                
                if (cardObject.StartHover()) {
                    hoveringOver = cardObject;
                    GeneratePreviewCards(cardObject);
                }

                if (cardObject.hoverReady) {
                    if (cardObject.card.type == CardType.TragicEvent) {
                        backgroundOffset += Time.deltaTime * new Vector2(0, 1);
                    } else if (cardObject.card.type == CardType.Event) {
                        backgroundOffset -= Time.deltaTime * new Vector2(0, 1);
                    } else if (cardObject.card.type == CardType.Choice) {
                        backgroundOffset -= Time.deltaTime * new Vector2(.5f, 0);
                    } else {
                        backgroundOffset = Vector2.zero;
                    }

                    background.mainTextureOffset = backgroundOffset;
                    FlashColor(Color.Lerp(cardObject.GetPrimaryColor(), backgroundColor, .7f), .1f);
                }

                if (Input.GetMouseButtonDown(0)) {
                    ActivateCard(cardObject);
                   // Debug.Log(cardObject.card.name);
                }
            }
        } else {
            if (hoveringOver != null) {
                hoveringOver.StopHover();
                SetBackGroundColorTarget(backgroundColor);
                HidePreviewCards();
            }

            hoveringOver = null;
        }
    }

    void TriggerTragicEvent(CardObject card) {
       // card.StartRotation()
    }

}
