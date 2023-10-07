using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCardNode : MonoBehaviour
{
    public int cardNum;
    public Image cardArea;
    public Text cardName;
    public int cardDmg;
    public Text cardDmgTxt;
    public int cardSP;
    public Text cardSPTxt;
    public List<Coords> cardCoords;
    static CardType cardType;
    public bool isSelected = false;

    Button cardBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card card)
    {
        cardType = card.cardType;
        cardNum = card.cardNum;
        cardName.text = card.cardName;
        cardDmg = card.cardDmg;
        cardDmgTxt.text = card.cardDmg.ToString();
        cardSP = card.cardSP;
        cardSPTxt.text = card.cardSP.ToString();
        cardArea.sprite = card.cardImage;

        if (card.coords.Length != 0)
        {
            for (int i = 0; i < card.coords.Length; i++)
            {
                cardCoords.Add(card.coords[i]);
            }
        }


        if (cardType == CardType.moveCard)
        {
            gameObject.tag = "MoveCard";
        }
        else if (cardType == CardType.attCard)
        {
            gameObject.tag = "AttCard";
        }
        else if (cardType == CardType.utilCard)
        {
            gameObject.tag = "UtilCard";
            cardSPTxt.text = "+" + card.cardSP.ToString();
        }
    }
}
