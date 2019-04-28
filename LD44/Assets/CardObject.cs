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

    float timer;
    public float timerMax = 1.0f;

    public Card card;

    public Color choiceColor;
    public Color choiceTextColor;

    public Sprite icon1;
    public Sprite icon2;
    public Sprite icon3;
    
    public TMPro.TextMeshProUGUI[] lineText = new TMPro.TextMeshProUGUI[3];


    public SpriteRenderer[] lineSprite = new SpriteRenderer[3];

    public void InitializeAnimation(Vector3 start, Vector3 target) {
        startPos = start;
        targetPos = target;
        timer = 0;
        transform.position = start;
    }

    public enum LineType
    {
        Choice
    };

    float startZ;

    void Awake() {
        startZ = transform.position.z;
        shadowBasePos = dropShadowFar.transform.localPosition;
    }
    public void InitializeLine(
            TMPro.TextMeshProUGUI text, 
            SpriteRenderer sprite,
            LineType lineType,
            string stringSet,
            int count
        )
    {
        if (lineType == LineType.Choice) {
            text.color = choiceTextColor;
            text.text = stringSet;
            sprite.color = choiceColor;
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
        }
    }

    private string CreateString(int count, string word) {
        string toAppend = "+" + card.choiceCards.Count.ToString() + " " + word;
        if (card.choiceCards.Count > 1) {toAppend += 's';}
        toAppend += "\n";
        return toAppend;
    }
    public void InitializeCard(Card cardSet) {

        timer = 0;
        hoverReady = false;
        hoverTimer = 0;

        card = cardSet;
        titleText.text = card.name;

        if (card.lifespan == 0) {
            lifespanText.text = "";
        } else {
            lifespanText.text = card.lifespan.ToString();
        }
        
        int currentLine = 0;
        
        if (card.choiceCards.Count > 0) {
            string toAppend = CreateString(card.choiceCards.Count, "Choice");
            InitializeLine(lineText[currentLine], lineSprite[currentLine], LineType.Choice, toAppend, card.choiceCards.Count);
            currentLine++;
        }

        for (int i = currentLine; i < 3; ++i) {
            lineText[i].enabled = false;
            lineSprite[i].enabled = false;
        }
        
    }
    

    bool hovering = false;

    float hoverOffset = -.03f;
    Vector3 originalPos;
    
    public bool hoverReady = false;

    public bool StartHover() {

        if (!hovering && hoverReady) {
            hovering = true;
           // hoverTargetPos = new Vector3(originalPos.x, originalPos.y, hoverOffset);
        }

        return hoverReady;
    }

    public void StopHover() {

        if (hovering) {
           // dropShadowClose.SetActive(true);
           // dropShadowFar.SetActive(false);
            hovering = false;
           // transform.position = originalPos;
        }
    }

    public Vector3 DropShadowMaxOffset = new Vector3(.25f, -.25f, 0);
    public Vector3 shadowBasePos;
    public float maxZ = -.1f;

    public float maxScale = 0.2f;

    public void updateShadow() {
        float amount = transform.position.z / maxZ;
        dropShadowContainer.localScale = new Vector3(1.0f, 1.0f,0) * (1.0f + maxScale * amount * 1.5f) + new Vector3(0, 0, 1);
       // dropShadowFar.transform.localPosition = Vector3.Lerp(shadowBasePos, shadowBasePos + DropShadowMaxOffset, amount); 
    }

    public AnimationCurve hoverAnimationCurve;
    float hoverTimer = 0;
    public float hoverTimerMax = .3f;

    public Transform dropShadowContainer;


    public void UpdateHover() {
        float amount = hoverAnimationCurve.Evaluate(hoverTimer / hoverTimerMax);
        transform.localScale = Vector3.one * (1.0f + (maxScale * amount));
        transform.position = new Vector3(transform.position.x, transform.position.y, 
        startZ + hoverOffset * amount);
    }

    public void Update() {
        
        if (!hovering) {
            transform.position = Vector3.Lerp(startPos, targetPos, animationCurve.Evaluate(timer / timerMax));
            timer += Time.deltaTime;

            if (hoverTimer > 0) {
                hoverTimer -= Time.deltaTime;
            }
        } else {
            if (hoverTimer < hoverTimerMax) {
                hoverTimer += Time.deltaTime * 3.0f;
            }
        }

        if (timer > timerMax && !hoverReady) {
            originalPos = transform.position;
            hoverReady = true;
        }

        
        UpdateHover();
        updateShadow();
    }

}
