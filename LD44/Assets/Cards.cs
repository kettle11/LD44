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

    public bool gainStar = false;
    public bool loseStar = false;

    public CardType type;

    public bool mystery = false;
    
    public bool randomCardback = false;

    public bool timeMachine = false;

    public Card(string nameSet, int lifespanSet) {
        name = nameSet;
        lifespan = lifespanSet;
        choiceCards = new List<Card>();
        tragicEvents = new List<Card>();
        events = new List<Card>();
    }

    public void AddCard(Card card) {

        if (card.randomOutcomes.Count > 0) {
            randomEvents.Add(card);
            return;
        }

        if(card.type == CardType.Event) {
            events.Add(card);
        }

        if(card.type == CardType.TragicEvent) {
            tragicEvents.Add(card);
        }

        if(card.type == CardType.Choice) {
            choiceCards.Add(card);
        }
    }
    public void AddChoiceCard(Card card) {
        AddCard(card);
    }

    public void AddEvent(Card card) {
        AddCard(card);
    }

    public void AddTragicEvent(Card card) {
        AddCard(card);
    }
    
    public static bool randomEnabled = false;
    public Card(Card card) {
        handSizeAdjustment = card.handSizeAdjustment;
        handSizeAdjustmentTurns = card.handSizeAdjustmentTurns;
        name = card.name;
        lifespan = card.lifespan;
        choiceCards = new List<Card>(card.choiceCards);
        tragicEvents = new List<Card>(card.tragicEvents);
        randomOutcomes = new List<Card>(card.randomOutcomes);
        randomEvents = new List<Card>(card.randomEvents);
        events = new List<Card>(card.events);
        type = card.type;
        cardDraw = card.cardDraw;
        cardDiscard = card.cardDiscard;
        mystery = card.mystery;
        randomCardback = card.randomCardback;
        gainStar = card.gainStar;
        loseStar = card.loseStar;
        timeMachine = card.timeMachine;
    }
    
    public List<Card> randomEvents = new List<Card>();
    
    public List<Card> randomOutcomes = new List<Card>();


    public void AddOutcome(Card card) {
        randomOutcomes.Add(card);
        randomCardback = randomCardback || card.type != CardType.Choice;
        mystery = true;
    }

    public Card GetRandomOutcome() {
        return randomOutcomes[Random.Range(0, randomOutcomes.Count)];
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

    public Card MakeCard(string name) {
        return MakeCard(name, 0);
    }

    public Card MakeTragicEvent(string name) {
        Card card = MakeCard(name, 0);
        card.type = CardType.TragicEvent;
        card.mystery = true;
        return card;
    }


    public Card MakeEvent(string name) {
        Card card = MakeCard(name, 0);
        card.type = CardType.Event;
        card.mystery = true;
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
        poopCard.cardDiscard = 1;
        Card sleepCard =  MakeCard("Sleep", 0);

        Card play1 = new Card("Play with crayons", 0);
        Card dumb0 = new Card("Eat crayons", 1);

        dumb0.AddChoiceCard(dumb0);
       // dumb0.AddChoiceCard(play1);
        
        play1.AddChoiceCard(dumb0);
       // play1.AddChoiceCard(dumb0);

        cry.AddChoiceCard(sleepCard);
        cry.AddChoiceCard(eatCard);
     //   cry.AddChoiceCard(burpCard);
        cry.AddCard(play1);

        eatCard.AddChoiceCard(poopCard);
        eatCard.cardDraw = 1;

        //poopCard.AddChoiceCard(sleepCard);
        
        //sleepCard.AddChoiceCard(eatCard);
        sleepCard.AddChoiceCard(play1);
        //sleepCard.cardDraw;

        Card firstDayOfSchool = MakeEvent("First day of school");
        
        Card rainbowPoo = MakeTragicEvent("Rainbow Poo");
        dumb0.AddTragicEvent(rainbowPoo);
        rainbowPoo.cardDiscard = 1;
        rainbowPoo.cardDraw = 1;

        Card icecream = MakeCard("Eat icecream", 1);
        icecream.AddCard(poopCard);
        icecream.cardDraw = 1;

        Card read = MakeCard("Learn to read", 2);

        firstDayOfSchool.AddCard(icecream);

        //graduate1.AddChoiceCard(pickupSport);
        //graduate1.AddChoiceCard(pickupBallet);

        Card dinosaurs = MakeCard("Read about dinosaurs and optical illusions", 1);
        dinosaurs.cardDraw = 1;
        firstDayOfSchool.AddCard(read);
        read.AddCard(dinosaurs);


        //Card study1 = MakeCard("Study more", 2);
        
       // Card insult0 = MakeCard("Call your friend a poo", 2);
       // Card sadFriend = MakeTragicEvent("Your friend is sad");

       // Card treefort = MakeCard("Make a sick treefort with your friend", 0);

        Card stealLunchMoney = MakeCard("Steal lunch money", 4);
        firstDayOfSchool.AddChoiceCard(stealLunchMoney);

        Card getYourLunchMoneyStolen = MakeTragicEvent("Get your lunch money stolen");
        getYourLunchMoneyStolen.loseStar = true;
        
        Card eatASinglePieceOfBread = MakeCard("Solemnly sip your juice box", 0);
        eatASinglePieceOfBread.cardDraw = 1;
        eatASinglePieceOfBread.cardDiscard = 1;

        Card stolenMoney = MakeCard("Use your stolen money");
        stolenMoney.cardDraw = 1;
        stolenMoney.gainStar = true;

        getYourLunchMoneyStolen.AddChoiceCard(eatASinglePieceOfBread);
        getYourLunchMoneyStolen.cardDiscard = 1;
        stealLunchMoney.AddTragicEvent(getYourLunchMoneyStolen);
        stealLunchMoney.AddCard(stolenMoney);
    
        Card injury = MakeTragicEvent("Suffer an injury");
        Card sick = MakeTragicEvent("You get sick");

        injury.cardDiscard = 4;

        sick.cardDiscard = 1;
        sick.AddTragicEvent(sick);

        Card winASportsTournament = MakeEvent("Your team wins a big tournament");
        winASportsTournament.gainStar = true;
        winASportsTournament.cardDraw = 3;
        winASportsTournament.cardDiscard = 1;

       // pickupBallet.AddEvent(winASportsTournament);

        Card sportChance = MakeCard("", 0);
        sportChance.AddOutcome(injury);
        sportChance.AddCard(winASportsTournament);

        //pickupSport.AddCard(sportChance);

        Card learnToCode = MakeCard("Learn to program", 6);
        Card hackTheGovernment = MakeCard("Hack a bank", 6);
        learnToCode.AddChoiceCard(hackTheGovernment);
        hackTheGovernment.AddEvent(stolenMoney);

        Card getArrested = MakeTragicEvent("Get arrested");
        getArrested.cardDiscard = 4;
        getArrested.cardDraw = 4;
        
        Card debt = MakeEvent("Pay debt");
        debt.cardDiscard = 1;
        debt.cardDraw = 1;
        debt.AddCard(debt);

        getArrested.AddCard(debt);

        hackTheGovernment.AddEvent(stolenMoney);
        hackTheGovernment.AddTragicEvent(getArrested);

        Card petADog = MakeCard("Pet a dog", 3);
        Card getADog = MakeCard("Adopt the dog", 3);
        
        Card randomName = MakeCard("nameDog");
        Card name0 = MakeCard("Name your dog Pudding", 0);

        randomName.AddOutcome(name0);
       // randomName.AddOutcome(name1);
       // randomName.AddOutcome(name2);

        Card dogLove = MakeCard("Your dog loves you");
        dogLove.gainStar = true;
        name0.AddEvent(dogLove);
       // name1.AddEvent(dogLove);
       // name2.AddCard(dogLove);

        Card petYourDog = MakeCard("Pet your dog");
        petYourDog.AddChoiceCard(petYourDog);
        dogLove.AddChoiceCard(petYourDog);

        petADog.AddChoiceCard(getADog);

        getADog.AddCard(randomName);
        //getADog.AddChoiceCard(name0);
       // getADog.AddChoiceCard(name1);
       // getADog.AddChoiceCard(name2);

        Card robABank = MakeCard("Rob a bank", 3);
        robABank.AddEvent(stolenMoney);
        robABank.AddTragicEvent(getArrested);

        Card college1 = MakeCard("Get a degree in interpretive dance", 3);
        Card college2 = MakeCard("Get a degree in really hard math", 3);

        college1.cardDiscard = 3;
        college1.gainStar = true;
        college2.cardDiscard = 3;
        college1.gainStar = true;
        college2.gainStar = true;

        college1.cardDraw = 3;
        college2.cardDraw = 3;

        //college1.AddChoiceCard(robABank);
        college2.AddChoiceCard(learnToCode);

        Card buyAHome = MakeCard("Buy a home", 8);
        buyAHome.gainStar = true;
        buyAHome.cardDiscard = 4;
        buyAHome.cardDraw = 1;
        Card buildAPool = MakeCard("Build a pool", 8);
        Card buyABoat = MakeCard("Buy a boat", 3);

        buyAHome.AddChoiceCard(buildAPool);
        buyAHome.AddChoiceCard(buyABoat);

        buyABoat.cardDiscard = 4;
        buyABoat.cardDraw = 1;

        Card meet0 = MakeCard("Say hi To Alfonso");
        Card meet1 = MakeCard("Ask Amelia to the dance");
        Card meet2 = MakeCard("Make eye contact with Muds");

        Card firstKiss0 = MakeCard("Share your first kiss with Alfonso", 3);
        Card firstKiss1 = MakeCard("Smooch Amelia", 3);
        Card firstKiss2 = MakeCard("Make out with Muds", 6);
        
        meet0.mystery = true;
        meet1.mystery = true;
        meet2.mystery = true;

        firstKiss0.mystery = true;
        firstKiss1.mystery = true;
        firstKiss2.mystery = true;

        Card dance = MakeEvent("Dance with Amelia") ;
        meet1.AddCard(dance);

        Card date0 = MakeCard("Date Alfonso", 3);
        Card date1 = MakeCard("Go out with Amelia", 3);
        Card date2 = MakeCard("Hang out under the bleachers with Muds", 3);
        
        date1.mystery = true;
        date2.mystery = true;

        meet0.AddCard(date0);
        meet2.AddCard(date2);
        dance.AddCard(date1);

        meet2.AddChoiceCard(robABank);

        Card spendMoney = MakeCard("Spend the big bucks", 2);
        spendMoney.cardDraw += 2;
        spendMoney.cardDiscard += 2;

        Card lottery = MakeEvent("You win the lottery!");
        lottery.AddChoiceCard(spendMoney);
        lottery.AddChoiceCard(spendMoney);
        
        lottery.AddChoiceCard(date0);
        lottery.AddChoiceCard(buyABoat);
        
       // college1.AddEvent(lottery);

        Card bankruptcy = MakeTragicEvent("You are bankrupt!");
        bankruptcy.AddChoiceCard(robABank);
        bankruptcy.AddChoiceCard(eatASinglePieceOfBread);
        bankruptcy.AddChoiceCard(date2);
        
        buildAPool.AddTragicEvent(bankruptcy);

        college2.AddEvent(bankruptcy);
        college2.AddChoiceCard(firstKiss2);

        firstKiss2.AddChoiceCard(firstKiss2);


        Card marry0 = MakeCard("Marry Alfonso");
        Card marry1 = MakeCard("Marry Amelia");
        Card marry2 = MakeCard("Move in with Muds");
        marry0.mystery = true;
        marry1.mystery = true;
        marry2.mystery = true;

        marry2.AddChoiceCard(robABank);
        marry2.AddCard(hackTheGovernment);
        
        date0.AddChoiceCard(marry0);

        Card vampire = MakeTragicEvent("Alfonso is a vampire");
        vampire.cardDiscard = 3;
        
        date0.AddTragicEvent(vampire);

       // randomCards.Add(bankruptcy);
        randomCards.Add(lottery);
        randomCards.Add(petADog);
        randomCards.Add(learnToCode);
        
        Card lotteryChance = MakeCard("lotteryChance", 0);
        lotteryChance.AddOutcome(lottery);
        lotteryChance.AddOutcome(rainbowPoo);

        Card acne = MakeTragicEvent("Acne");
        acne.cardDiscard = 2;
        
        marry2.AddCard(acne);

        Card insecurity = MakeEvent("Insecurity");
        insecurity.cardDiscard = 1;
        insecurity.mystery = false;
        acne.AddCard(insecurity);
        

        Card moreAcne = MakeTragicEvent("More Acne");
        moreAcne.cardDiscard = 2;
        moreAcne.mystery = false;

        Card trySomethingNew = MakeCard("Try something new");

        Card tryOutForSports = MakeCard("Try out for a sports team");
        Card joinDramaClub = MakeCard("Join drama club");

        Card makeSportsTeamChance = MakeCard("makeSportsTeamChance");
        Card makeSportsTeam = MakeEvent("You make it onto the sports team!");
        makeSportsTeam.cardDraw = 1;
        makeSportsTeamChance.AddOutcome(makeSportsTeam);
        Card rejectFromSportsTeam = MakeTragicEvent("You fail at sports tryouts.");
        makeSportsTeamChance.AddOutcome(rejectFromSportsTeam);

        tryOutForSports.AddChoiceCard(makeSportsTeamChance);

        Card sportsChances = MakeEvent("sportsChances");
        sportsChances.AddOutcome(injury);
        sportsChances.AddOutcome(winASportsTournament);

        makeSportsTeam.AddCard(sportsChances);

        rejectFromSportsTeam.AddEvent(insecurity);
        trySomethingNew.AddChoiceCard(tryOutForSports);
        //trySomethingNew.AddCard(pickupBallet);
        trySomethingNew.cardDraw = 1;
        trySomethingNew.AddCard(joinDramaClub);
        rejectFromSportsTeam.AddChoiceCard(trySomethingNew);


        insecurity.AddCard(moreAcne);
        moreAcne.AddCard(insecurity);

        Card puberty = MakeEvent("Puberty");
        puberty.gainStar = true;
        //puberty.AddCard(petADog);

        Card wakeUpEarly = MakeTragicEvent("Wake up at 6 am to go to school");
        wakeUpEarly.AddTragicEvent(sick);

        wakeUpEarly.cardDraw = 2;
        wakeUpEarly.cardDiscard = 1;
        puberty.AddTragicEvent(wakeUpEarly);
    
        puberty.AddCard(trySomethingNew);
        puberty.AddTragicEvent(acne);
        puberty.AddCard(meet0);
        puberty.AddCard(meet1);
        puberty.AddCard(meet2);

        Card buyLotteryTicket = MakeCard("Buy a lottery ticket");
        buyLotteryTicket.cardDiscard = 1;
        buyLotteryTicket.cardDraw = 1;
        buyLotteryTicket.AddCard(lotteryChance);
        
        Card dramaClubOutcomes = MakeCard("dramaClubOutcomes");
        Card starInShow = MakeCard("You play the leading giraffe in Othello"); 
        Card sideCharacter = MakeCard("You play a background hippo in the school play"); 
        
        starInShow.gainStar = true;
        dramaClubOutcomes.AddOutcome(starInShow);
        dramaClubOutcomes.AddOutcome(sideCharacter);
        
        Card wellReviewed = MakeEvent("Your performance as a hippo gets rave reviews");
        sideCharacter.AddCard(wellReviewed);
        sideCharacter.AddCard(insecurity);

        joinDramaClub.AddCard(insecurity);
        joinDramaClub.AddCard(dramaClubOutcomes);
    
    
        //birth.AddCard(acne);

        birth.gainStar = true;


        // College life stuff
        Card turn18 = MakeEvent("Adulthood");
        turn18.AddCard(petADog);

        firstDayOfSchool.AddCard(petADog);

        turn18.gainStar = true;
        Card fileTaxes = MakeEvent("File taxes");
        fileTaxes.loseStar = true;
        fileTaxes.cardDiscard = 2;
        turn18.AddCard(buyLotteryTicket);
        turn18.AddCard(fileTaxes);

        turn18.AddCard(college1);
        turn18.AddCard(college2);

        turn18.AddChoiceCard(marry0);
        turn18.AddChoiceCard(marry1);
        turn18.AddChoiceCard(marry2);

        turn18.AddCard(buyABoat);
        turn18.AddCard(buyAHome);

        Card turn40 = MakeEvent("Midlife");
        turn40.gainStar = true;
        turn40.AddCard(buyABoat);
        turn40.AddCard(buyAHome);

        //bankruptcy.AddCard(debt);

        college1.AddCard(debt);
        college2.AddCard(debt);

        dance.AddCard(insecurity);
        
        Card timeMachine = MakeEvent("A strange machine makes you a baby again");
        
        timeMachine.AddCard(cry);
        timeMachine.AddCard(sleepCard);

    }

    public Cards() {
        CreateCards();
    }
}