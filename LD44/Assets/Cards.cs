using System.Collections.Generic;

public enum CardType
{
    Choice,
    TragicEvent
};

public class Card
{
    public string name;
    
    public int lifespan;

    public bool onetime = false;

    public List<Card> choiceCards;
    public List<Card> tragicEvents;


    public CardType type;

    public Card(string nameSet, int lifespanSet) {
        name = nameSet;
        lifespan = lifespanSet;
        choiceCards = new List<Card>();
        tragicEvents = new List<Card>();
    }

    public void AddChoiceCard(Card card) {
        choiceCards.Add(card);
    }

    public void AddTragicEvent(Card card) {
        tragicEvents.Add(card);
    }

    public Card(Card card) {
        name = card.name;
        lifespan = card.lifespan;
        choiceCards = new List<Card>(card.choiceCards);
        tragicEvents = new List<Card>(card.tragicEvents);
        type = card.type;
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

    public Card MakeTragicEvent(string name, int lifespan) {
        Card card = MakeCard(name, lifespan);
        card.type = CardType.TragicEvent;
        return card;
    }

    public void OneTime(Card card) {
        hasBeenPlayed[card] = true;
    }

    public bool CheckPlayed(Card card) {
        return hasBeenPlayed.ContainsKey(card);
    }

    public void CreateCards() {
        Card testTragedy = MakeTragicEvent("Test Tragedy", 0);

        Card eatCard = MakeCard("Eat", 0);
        Card burpCard = MakeCard("Burp", 0);

        Card cry = MakeCard("Cry", 0);

        cry.AddTragicEvent(testTragedy);

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