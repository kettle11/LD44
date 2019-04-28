using System.Collections.Generic;

public enum CardType
{
    Choice,
    TragicEvent,
    Event
};

public class Card
{
    public string name;
    
    public int lifespan;

    public bool onetime = false;

    public List<Card> choiceCards;
    public List<Card> tragicEvents;
    public List<Card> events;

    public int handSizeAdjustmentTurns;
    public int handSizeAdjustment; 

    public int cardDraw = 0;
    public int cardDiscard = 0;

    public CardType type;

    public Card(string nameSet, int lifespanSet) {
        name = nameSet;
        lifespan = lifespanSet;
        choiceCards = new List<Card>();
        tragicEvents = new List<Card>();
        events = new List<Card>();
    }

    public void AddChoiceCard(Card card) {
        choiceCards.Add(card);
    }

    public void AddEvent(Card card) {
        events.Add(card);
    }

    public void AddTragicEvent(Card card) {
        tragicEvents.Add(card);
    }

    public Card(Card card) {
        handSizeAdjustment = card.handSizeAdjustment;
        handSizeAdjustmentTurns = card.handSizeAdjustmentTurns;
        name = card.name;
        lifespan = card.lifespan;
        choiceCards = new List<Card>(card.choiceCards);
        tragicEvents = new List<Card>(card.tragicEvents);
        events = new List<Card>(card.events);
        type = card.type;
        cardDraw = card.cardDraw;
        cardDiscard = card.cardDiscard;
    }
}

public class Cards
{
    public Dictionary<string, Card> cards = new Dictionary<string, Card>();
    public Dictionary<Card, bool> hasBeenPlayed = new Dictionary<Card, bool>();

    public Card MakeCard(string name, int lifespan) {
        Card newCard = new Card(name, lifespan);
        newCard.type = CardType.Choice;
        cards.Add(name, newCard);
        return newCard;
    }

    public Card MakeTragicEvent(string name) {
        Card card = MakeCard(name, 0);
        card.type = CardType.TragicEvent;
        return card;
    }


    public Card MakeEvent(string name) {
        Card card = MakeCard(name, 0);
        card.type = CardType.Event;
        return card;
    }

    public void OneTime(Card card) {
        hasBeenPlayed[card] = true;
    }

    public bool CheckPlayed(Card card) {
        return hasBeenPlayed.ContainsKey(card);
    }

    public void CreateCards() {
        Card birth = MakeEvent("Birth");

        birth.cardDiscard = 2;
        birth.cardDraw = 2;
        Card testTragedy = MakeTragicEvent("Smelly Socks");
        testTragedy.handSizeAdjustment = 3;
        testTragedy.handSizeAdjustmentTurns = 3;

        Card eatCard = MakeCard("Eat", 2);
        Card burpCard = MakeCard("Burp", 0);

        Card cry = MakeCard("Cry", 0);

        cry.AddEvent(birth);

       // cry.AddTragicEvent(testTragedy);

        Card poopCard =  MakeCard("Poo", 0);
        Card sleepCard =  MakeCard("Sleep", 0);

        Card play0 = new Card("Play with toys", 0);
        Card play1 = new Card("Play with crayons", 0);
        Card dumb0 = new Card("Eat crayons", 1);

        dumb0.AddChoiceCard(poopCard);
        dumb0.AddChoiceCard(dumb0);

        Card draw = new Card("Draw a picture", 0);

        play1.AddChoiceCard(dumb0);
        play1.AddChoiceCard(draw);
        play0.AddChoiceCard(play1);

        cry.AddChoiceCard(sleepCard);
        cry.AddChoiceCard(eatCard);
        cry.AddChoiceCard(cry);
        
        eatCard.AddChoiceCard(poopCard);
        eatCard.AddChoiceCard(burpCard);

        poopCard.AddChoiceCard(eatCard);
        poopCard.AddChoiceCard(sleepCard);
        
        sleepCard.AddChoiceCard(play0);

        Card school = new Card("Attend first day of school", 1);
        school.onetime = true;

        draw.AddChoiceCard(school);

        Card makeAFriend = new Card("Make your first friend", 1);
        makeAFriend.onetime = true;

        Card graduate1 = new Card("Graduate from the 1st grade", 2);

        school.AddChoiceCard(makeAFriend);
        school.AddChoiceCard(graduate1);
        
        Card pickupBasketball = new Card("Play basketball", 0);
        Card pickupSoccer = new Card("Play soccer", 0);
        Card pickupBallet = new Card("Start ballet", 0);

        graduate1.AddChoiceCard(pickupBasketball);
        graduate1.AddChoiceCard(pickupSoccer);
        graduate1.AddChoiceCard(pickupBallet);

        Card study0 = new Card("Study", 1);

        graduate1.AddChoiceCard(study0);
    }

    public Cards() {
        CreateCards();
    }
}