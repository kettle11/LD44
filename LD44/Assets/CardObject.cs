using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{   
    public TMPro.TextMeshProUGUI titleText;

    public TMPro.TextMeshProUGUI lifespanText;

    public Vector3 targetPos;
    public Vector3 startPos;

    public Quaternion targetRotation;

    public AnimationCurve animationCurve;

    public GameObject dropShadowClose;
    public GameObject dropShadowFar;

    public float timer;
    public float timerMax = 1.0f;

    public Card card;

    public Color choiceColor;
    public Color choiceTextColor;

    public Texture2D choiceBackMaterial;
    public Texture2D choiceFrontTexture;

    public Color tragicEventColor;
    public Color tragicEventTextColor;
    
    public Texture2D tragicEventBackMaterial;
    public Texture2D tragicEventFrontTexture;

    public Color eventcolor;
    public Color eventTextColor;
    
    public Texture2D eventBackMaterial;
    public Texture2D eventFrontTexture;

    public Texture2D mysteryEventBackMaterial;
    
    public MeshRenderer frontRenderer;

    public Sprite icon1;
    public Sprite icon2;
    public Sprite icon3;
    
    public TMPro.TextMeshProUGUI[] lineText = new TMPro.TextMeshProUGUI[3];


    public SpriteRenderer[] lineSprite = new SpriteRenderer[3];

    public Color discardTextColor;
    public Color drawTextColor;
    
    public void InitializeAnimation(float timerSet, Vector3 start, Vector3 target) {
        startPos = start;
        targetPos = target;
        timer = timerSet;
        rotationTimer = 0;
        hoverReady = false;

        transform.position = start;
    }

    public enum LineType
    {
        Choice,
        TragicEvent,
        Event, 
        Discard,
        Draw
    };

    float startZ;
    public float baseScale;
    float actualBaseScale;

    public int handIndex;
    public MeshRenderer backRenderer;
    public MeshRenderer shadowRenderer;

    TMPro.TextMeshProUGUI[] allText;
    MeshRenderer[] allRenderers;

    void Awake() {
        startZ = transform.position.z;
        shadowBasePos = dropShadowFar.transform.localPosition;
        actualBaseScale = transform.localScale.x;

        backRenderer.material = new Material(backRenderer.material);
        shadowRenderer.material = new Material(shadowRenderer.material);

        backMaterial = backRenderer.material;
        dropShaderMaterial = shadowRenderer.material;
        allText = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        allRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void SetAlpha(float alpha) {
        foreach(TMPro.TextMeshProUGUI text in allText) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }

        foreach(MeshRenderer renderer in allRenderers) {
            Color c = renderer.material.color;
            renderer.material.color = new Color(c.r, c.g, c.b, alpha);
        }

        foreach(SpriteRenderer r in lineSprite) {
            r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
        }
    }

    public Color GetPrimaryColor() {

        if (card == null) {
            return choiceColor;
        }

        if (card.type == CardType.Choice) {
            return choiceColor;
        } else if (card.type == CardType.TragicEvent) {
            return tragicEventColor;
        } else if (card.type == CardType.Event) {
            return eventcolor;
        }

        return choiceColor;
    }

    public void InitializeLine(
            TMPro.TextMeshProUGUI text, 
            SpriteRenderer sprite,
            LineType lineType,
            string stringSet,
            int count
        )
    {
        text.text = stringSet;
        if (lineType == LineType.Choice) {
            text.color = choiceTextColor;
            sprite.color = choiceColor;
        } else if (lineType == LineType.TragicEvent) {
            text.color = tragicEventTextColor;
            sprite.color = tragicEventColor;
        } else if (lineType == LineType.Event) {
            text.color = eventTextColor;
            sprite.color = eventcolor;
        }

        if (count == 1) {
            sprite.sprite = icon1;
        } else if (count == 2) {
            sprite.sprite = icon2;
        } else {
            sprite.sprite = icon3;
        }

        text.enabled = true;

        if (count > 0) {
            sprite.enabled = true;
        } else {
            sprite.enabled = false;
        }

        if(lineType == LineType.Draw) {
            text.color = drawTextColor;
        }

        if(lineType == LineType.Discard) {
            text.color = discardTextColor;
        }
    }

    private string CreateString(int count, string word) {
        string toAppend = "+" + count.ToString() + " " + word;
        if (count> 1) {toAppend += 's';}
        toAppend += "\n";
        return toAppend;
    }
    
    public bool flipped = false;

    public GameObject gainStar;
    public GameObject loseStar;
    public void InitializeCard(Card cardSet) {

        gainStar.SetActive(cardSet.gainStar);
        loseStar.SetActive(cardSet.loseStar);

        SetAlpha(1.0f);
        decrementDeckCountWhenStart = false;
        incrementDeckCountWhenDone = false;
        waitingForAnimation = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        returnWhenAnimationDone = false;
        runFadeOut = false;
        runRotate = false;
        isPreview = false;
        baseScale = actualBaseScale;

        backMaterial.color = new Color(1, 1, 1, 1);
        dropShaderMaterial.color = new Color(1, 1, 1, 1);

        timer = 0;
        hoverReady = false;
        hoverTimer = 0;

        card = cardSet;
        titleText.text = card.name;

        if (card.lifespan == 0 || true) {
            lifespanText.text = "";
        } else {
            lifespanText.text = card.lifespan.ToString();
        }
        
    
        int currentLine = 0;

       // if(card.type == CardType.Choice && card.tragicEvents.Count == 0 && card.events.Count == 0) currentLine = 1;
        
        for (int i = 0; i < 3; ++i) {
            lineText[i].enabled = false;
            lineSprite[i].enabled = false;
        }

        if (card.type == CardType.Choice) {
            backMaterial.mainTexture = choiceBackMaterial;
            frontRenderer.material.mainTexture = choiceFrontTexture;
        } else if (card.type == CardType.TragicEvent) {
            backMaterial.mainTexture = tragicEventBackMaterial;
            frontRenderer.material.mainTexture = tragicEventFrontTexture;
        } else if (card.type == CardType.Event) {
            backMaterial.mainTexture = eventBackMaterial;
            frontRenderer.material.mainTexture = eventFrontTexture;
        }   

        if (card.randomCardback) {
            backMaterial.mainTexture = mysteryEventBackMaterial;
        }
    
        if (card.choiceCards.Count > 0) {
            string toAppend = CreateString(card.choiceCards.Count, "Choice");
            InitializeLine(lineText[currentLine], lineSprite[currentLine], LineType.Choice, toAppend, card.choiceCards.Count);
            lineText[currentLine].enabled = true;
            lineSprite[currentLine].enabled = true;
            currentLine++;
        }

        if (card.tragicEvents.Count > 0) {
            int count = card.tragicEvents.Count;
            string toAppend = CreateString(count, "Tragic Event");
            InitializeLine(lineText[currentLine], lineSprite[currentLine], LineType.TragicEvent, toAppend, count);

            lineText[currentLine].enabled = true;
            lineSprite[currentLine].enabled = true;
            currentLine++;
        }

        if (card.events.Count > 0) {
            int count = card.events.Count;
            string toAppend = CreateString(count, "Event");
            InitializeLine(lineText[currentLine], lineSprite[currentLine], LineType.Event, toAppend, count);

            lineText[currentLine].enabled = true;
            lineSprite[currentLine].enabled = true;
            currentLine++;
        }

        if (currentLine > 2) return;

        if (card.randomEvents.Count > 0) {
            int count = card.randomEvents.Count;
            string toAppend = CreateString(count, "Mystery");
            InitializeLine(lineText[currentLine], lineSprite[currentLine], LineType.Event, toAppend, count);

            lineText[currentLine].enabled = true;
            lineSprite[currentLine].enabled = true;
            currentLine++;
        }

        if (currentLine > 2) return;

        if (card.handSizeAdjustmentTurns > 0) {
            string toAppend;
            
            if(card.handSizeAdjustmentTurns == 1) {
                toAppend = "Next time you draw " + card.handSizeAdjustment.ToString() + "you only draw cards";
            } else {
                toAppend = "For " + card.handSizeAdjustmentTurns.ToString() + "draws you only draw "
                 + card.handSizeAdjustment.ToString() + " cards";
            }

            InitializeLine(lineText[currentLine], lineSprite[currentLine], LineType.TragicEvent, toAppend, 0);

            lineText[currentLine].enabled = true;
          //  lineSprite[currentLine].enabled = true;
            currentLine++;
        }
        
        if (currentLine > 2) return;

         if (card.cardDiscard > 0) {
            LineType lineType = LineType.Discard;

             string toAppend;
            
            if(card.cardDiscard == 1) {
                toAppend = "Discard 1 card";
            } else {
                toAppend = "Discard " + card.cardDiscard.ToString() + " cards";
            }

            InitializeLine(lineText[currentLine], lineSprite[currentLine], lineType, toAppend, 0);

            lineText[currentLine].enabled = true;
            currentLine++;
        }

        if (currentLine > 2) return;

        if (card.cardDraw > 0) {
            LineType lineType = LineType.Draw;

             string toAppend;
            
            if(card.cardDraw == 1) {
                toAppend = "Draw 1 card";
            } else {
                toAppend = "Draw " + card.cardDraw.ToString() + " cards";
            }

            InitializeLine(lineText[currentLine], lineSprite[currentLine], lineType, toAppend, 0);

            lineText[currentLine].enabled = true;
            currentLine++;
        }

    }
    

    bool hovering = false;

    float hoverOffset = -.03f;
    Vector3 originalPos;
    
    public bool hoverReady = false;

    public bool isPreview = false;
    public bool StartHover() {

        bool startedHover = false;
        if (!hovering && hoverReady) {
            hovering = true;
            startedHover = true;
           // hoverTargetPos = new Vector3(originalPos.x, originalPos.y, hoverOffset);
        }

        return startedHover;
    }

    public void StopHover() {

        if (hovering) {
           // dropShadowClose.SetActive(true);
           // dropShadowFar.SetActive(false);
            hovering = false;
           // transform.position = originalPos;
        }
    }

    public bool returnWhenAnimationDone = false;

    public Vector3 DropShadowMaxOffset = new Vector3(.25f, -.25f, 0);
    public Vector3 shadowBasePos;
    public float maxZ = -.1f;

    public float maxScale = 0.2f;
    
    public bool incrementDeckCountWhenDone;
    public bool decrementDeckCountWhenStart;

    public void updateShadow() {
        float amount = transform.position.z / maxZ;
        dropShadowContainer.localScale = new Vector3(1.0f, 1.0f,0) * (1.0f + maxScale * amount * 1.5f) + new Vector3(0, 0, 1);
       // dropShadowFar.transform.localPosition = Vector3.Lerp(shadowBasePos, shadowBasePos + DropShadowMaxOffset, amount); 
    }

    public AnimationCurve hoverAnimationCurve;
    public float hoverTimer = 0;
    public float hoverTimerMax = .3f;

    public Transform dropShadowContainer;


    public void UpdateHover() {
        float amount = hoverAnimationCurve.Evaluate(hoverTimer / hoverTimerMax);
        transform.localScale = Vector3.one * (baseScale + (maxScale * amount));
        transform.position = new Vector3(transform.position.x, transform.position.y, 
        startZ + hoverOffset * amount);
    }

    public AnimationCurve rotationCurve;
    public float targetRotationDegrees = 0;

    public float startRotation = 0;

    public float rotationTimer = 0;
    public float rotationTimerMax = .4f;

    public bool runRotate = false;
    public void StartRotation(float timerStart, float start, float end) {
        runRotate = true;
        startRotation = start;
        rotationTimer = timerStart;
        transform.rotation = Quaternion.Euler(0, start, 0);
    }

    public float fadeTimer;
    public float fadeTimerMax;
    
    public Material backMaterial;
    public Material dropShaderMaterial;

    bool runFadeOut = false;
    public void StartFadeOut(float timerSet) {
        fadeTimer = timerSet;
        runFadeOut = true;
    }

    public void UpdateFadeOut() {

        if (fadeTimer > 0 && runFadeOut) {
            float alpha = 1.0f - Mathf.Clamp(fadeTimer / fadeTimerMax, 0, 1.0f);
            SetAlpha(alpha);
            /*
            backMaterial.color = new Color(1, 1, 1, alpha);
            dropShaderMaterial.color = new Color(1, 1, 1, alpha);*/
        }

        fadeTimer += Time.deltaTime;
    }

    public bool waitingForAnimation = false;

    public Deck deck;

    // used to flip cards over
    void UpdateRotation() {
        if (!runRotate) return;
        if (rotationTimer > 0) {

            float angle = 
            (targetRotationDegrees - startRotation)
             * rotationCurve.Evaluate(rotationTimer / rotationTimerMax)
              + startRotation;

            transform.rotation = Quaternion.Euler(0,angle, 0);
        }

        if (rotationTimer > rotationTimerMax) {
            runRotate = false;
            rotationTimer = 0;
        }

        rotationTimer += Time.deltaTime;
    }
    
    void Update() {
        if (!hovering) {

            if (timer > 0) {
                if (decrementDeckCountWhenStart) {
                    decrementDeckCountWhenStart = false;
                    deck.visibleDeckSize--;
                    deck.UpdateDeckSize();
                    //Debug.Log("-1 to Deck Size: " + card.name);
                }
                transform.position = Vector3.Lerp(startPos, targetPos, animationCurve.Evaluate(timer / timerMax));
            }

            timer += Time.deltaTime;
            
            if (hoverTimer > 0) {
                hoverTimer -= Time.deltaTime;
            }
        } else {
            if (hoverTimer < hoverTimerMax) {
                hoverTimer += Time.deltaTime * 3.0f;
            }
        }

        if (timer > timerMax && !hoverReady && rotationTimer > rotationTimerMax) {
            originalPos = transform.position;
            hoverReady = true;
        }

        if (returnWhenAnimationDone && timer > timerMax && fadeTimer > fadeTimerMax) {
            deck.ReturnCardToPool(this);
        }

       // transform.position += Vector3.right * .001f;
        
        UpdateHover();
        updateShadow();
        UpdateRotation();
        UpdateFadeOut();
    }

}
