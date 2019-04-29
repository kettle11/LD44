using System.Collections.Generic;
using UnityEngine;
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
    
    public static bool randomEnabled = false;
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

    public static List<Card> randomCards = new List<Card>();
    
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
        Card birth = MakeCard("Begin your life", 0);
        
        /*
        Card testTragedy = MakeTragicEvent("Smelly Socks");
        testTragedy.handSizeAdjustment = 3;
        testTragedy.handSizeAdjustmentTurns = 3;
        */

        Card eatCard = MakeCard("Eat", 0);
        Card burpCard = MakeCard("Burp", 0);

        Card cry = MakeCard("Cry", 0);

        birth.AddChoiceCard(cry);
        birth.AddChoiceCard(burpCard);

       // cry.AddTragicEvent(testTragedy);

        Card poopCard =  MakeCard("Poo", 0);
        Card sleepCard =  MakeCard("Sleep", 0);

        Card play1 = new Card("Play with crayons", 0);
        Card dumb0 = new Card("Eat crayons", 1);

        dumb0.AddChoiceCard(poopCard);
        dumb0.AddChoiceCard(dumb0);
        dumb0.AddChoiceCard(play1);
        
        Card draw = new Card("Draw a picture", 0);

        play1.AddChoiceCard(dumb0);
        play1.AddChoiceCard(dumb0);
        play1.AddChoiceCard(draw);

        cry.AddChoiceCard(sleepCard);
        cry.AddChoiceCard(eatCard);
        cry.AddChoiceCard(burpCard);

        eatCard.AddChoiceCard(poopCard);

        poopCard.AddChoiceCard(sleepCard);
        
        sleepCard.AddChoiceCard(eatCard);
        sleepCard.AddChoiceCard(play1);
        
        Card rainbowPoo = MakeTragicEvent("Rainbow Poo");
        dumb0.AddTragicEvent(rainbowPoo);
        rainbowPoo.cardDiscard = 1;
        rainbowPoo.cardDraw = 1;

        Card school = MakeCard("Attend school", 1);
        school.onetime = true;

        draw.AddChoiceCard(school);

        Card makeAFriend = MakeCard("Make a friend", 1);
        makeAFriend.onetime = true;

        Card graduate1 = MakeCard("Learn to read", 2);

        school.AddChoiceCard(makeAFriend);
        school.AddChoiceCard(graduate1);
        
        Card pickupSport = new Card("Play a sport", 0);
        Card pickupBallet = new Card("Start ballet", 0);

        graduate1.AddChoiceCard(pickupSport);
        graduate1.AddChoiceCard(pickupBallet);

        Card study0 = MakeCard("Study", 1);
        Card study1 = MakeCard("Study more", 2);
        
        Card insult0 = MakeCard("Call your friend a poo", 2);
        Card sadFriend = MakeTragicEvent("Your friend is sad");
        insult0.AddTragicEvent(sadFriend);

        Card treefort = MakeCard("Make a sick treefort with your friend", 0);

        makeAFriend.AddChoiceCard(insult0);
        makeAFriend.AddChoiceCard(treefort);

        Card stealLunchMoney = MakeCard("Steal lunch money", 4);
        study0.AddChoiceCard(stealLunchMoney);
        study1.AddChoiceCard(study1);

        Card graduateElementarySchool = MakeCard("Graduate Elementary School", 3);
        study1.AddChoiceCard(graduateElementarySchool);
        Card getYourLunchMoneyStolen = MakeTragicEvent("Get your lunch money stolen");

        Card eatASinglePieceOfBread = MakeCard("Eat a single piece of bread for lunch", 0);

        insult0.AddChoiceCard(eatASinglePieceOfBread);
        insult0.AddChoiceCard(study1);

        Card stolenMoney = MakeEvent("Use your stolen money");
        stolenMoney.cardDraw = 1;

        getYourLunchMoneyStolen.AddChoiceCard(eatASinglePieceOfBread);
        getYourLunchMoneyStolen.cardDiscard = 1;
        stealLunchMoney.AddTragicEvent(getYourLunchMoneyStolen);
        stealLunchMoney.cardDiscard = 1;
        stealLunchMoney.AddEvent(stolenMoney);

        graduate1.AddChoiceCard(study0);

        Card injury = MakeTragicEvent("Suffer an injury");
        Card sick = MakeTragicEvent("Get sick");

        injury.handSizeAdjustment = 1;
        injury.handSizeAdjustment = 2;

        sick.handSizeAdjustment = 3;
        sick.handSizeAdjustmentTurns = 4;

        graduateElementarySchool.AddTragicEvent(injury);

        Card playMoreSports = MakeCard("Play more sports", 2);
        Card winASportsTournament = MakeEvent("Win a competition");
        winASportsTournament.cardDraw = 3;
        winASportsTournament.cardDiscard = 1;

        pickupBallet.AddEvent(winASportsTournament);
        pickupSport.AddChoiceCard(playMoreSports);

        playMoreSports.AddChoiceCard(winASportsTournament);
        playMoreSports.AddTragicEvent(injury);

        graduateElementarySchool.AddChoiceCard(playMoreSports);

        Card learnToCode = MakeCard("Learn to program", 6);
        Card hackTheGovernment = MakeCard("Hack a bank", 6);
        learnToCode.AddChoiceCard(hackTheGovernment);
        hackTheGovernment.AddEvent(stolenMoney);

        Card getArrested = MakeTragicEvent("Get arrested");
        getArrested.cardDiscard = 2;
        getArrested.handSizeAdjustment = 1;
        getArrested.handSizeAdjustmentTurns = 4;

        hackTheGovernment.AddEvent(stolenMoney);
        hackTheGovernment.AddTragicEvent(getArrested);

        Card petADog = MakeCard("Pet a dog", 3);
        Card getADog = MakeCard("Adopt the dog", 3);
        Card name0 = MakeCard("Name your dog Binky", 0);
        Card name1 = MakeCard("Name your dog Blooper", 0);
        Card name2 = MakeCard("Name your dog Betsy", 0);

        petADog.AddChoiceCard(getADog);
        getADog.AddChoiceCard(name0);
        getADog.AddChoiceCard(name1);
        getADog.AddChoiceCard(name2);

        Card robABank = MakeCard("Rob a bank", 3);
        robABank.AddEvent(stolenMoney);
        robABank.AddTragicEvent(getArrested);

        Card college0 = MakeCard("Get a degree in marine biology", 3);
        Card college1 = MakeCard("Get a degree interpretive dance", 3);
        Card college2 = MakeCard("Get a degree in really hard math", 3);

        college2.AddChoiceCard(robABank);
        college2.AddChoiceCard(learnToCode);

        Card buyAHome = MakeCard("Buy a home", 8);
        buyAHome.cardDiscard = 1;

        Card buildAPool = MakeCard("Build a pool", 8);
        Card buyABoat = MakeCard("Buy a boat", 3);

        buyAHome.AddChoiceCard(buildAPool);
        buyAHome.AddChoiceCard(buyABoat);

        buyABoat.cardDiscard = 2;

        college0.AddChoiceCard(buyABoat);

        Card firstKiss0 = MakeCard("Share your first kiss with Alfonso", 3);
        Card firstKiss1 = MakeCard("Smooch Amelia", 3);
        Card firstKiss2 = MakeCard("Make out with Muds", 6);
        
        Card date0 = MakeCard("Date Alfonso", 3);
        Card data1 = MakeCard("Go out with Amelia", 3);
        Card data2 = MakeCard("Run away with Muds", 3);

        Card spendMoney = MakeCard("Spend the big bucks", 2);
        spendMoney.cardDraw += 2;
        spendMoney.cardDiscard += 2;

        Card lottery = MakeEvent("You win the lottery!");
        lottery.AddChoiceCard(spendMoney);
        lottery.AddChoiceCard(spendMoney);
        
        lottery.AddChoiceCard(date0);
        lottery.AddChoiceCard(buyABoat);
        
        college1.AddEvent(lottery);

        Card bankruptcy = MakeTragicEvent("You are bankrupt!");
        bankruptcy.AddChoiceCard(robABank);
        bankruptcy.AddChoiceCard(eatASinglePieceOfBread);
        bankruptcy.AddChoiceCard(data2);

        buildAPool.AddTragicEvent(bankruptcy);

        college2.AddEvent(bankruptcy);
        college2.AddChoiceCard(firstKiss2);
        
        firstKiss0.AddChoiceCard(date0);
        firstKiss0.AddChoiceCard(data1);
        firstKiss1.AddChoiceCard(data1);
        firstKiss1.AddChoiceCard(data2);

        firstKiss2.AddChoiceCard(firstKiss2);

        data2.AddChoiceCard(robABank);
        data2.AddChoiceCard(hackTheGovernment);

        date0.AddChoiceCard(MakeCard("Marry Alfonso", 3));;
        data1.AddChoiceCard(MakeCard("Marry Amelia", 3));
        data2.AddChoiceCard(MakeCard("Spend time with Muds", 3));
        
        Card vampire = MakeTragicEvent("Alfonso is a vampire");
        vampire.cardDiscard = 3;
        
        date0.AddTragicEvent(vampire);

       // randomCards.Add(bankruptcy);
        randomCards.Add(lottery);
        randomCards.Add(petADog);
        randomCards.Add(learnToCode);
        
    }

    public Cards() {
        CreateCards();
    }
}