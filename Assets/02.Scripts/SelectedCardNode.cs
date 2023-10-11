using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCardNode : MonoBehaviour
{
    public int cardKey;
    public int cardNum;
    public Image cardArea;
    public Text cardName;
    public int cardDmg;
    public Text cardDmgTxt;
    public int cardSP;
    public Text cardSPTxt;
    public List<Coords> cardCoords;
    static CardType cardType;

    Button selectedCardBtn;

    // Start is called before the first frame update
    void Start()
    {
        selectedCardBtn = GetComponent<Button>();

        if(selectedCardBtn != null )
        {
            selectedCardBtn.onClick.AddListener(CancleCard);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card card, int key)
    {
        cardKey = key;
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

    void CancleCard()
    {
        //선택된 카드 순서 리스트에서 key값이 일치하는 카드 찾아서 제거
        for(int i = 0; i < BattleMgr.selectedCardOrder.Count; i++)
        {
            int key = BattleMgr.selectedCardOrder[i].GetComponent<CardMgr>().cardKey;
            if(key == cardKey)
            {
                BattleMgr.selectedCardOrder.Remove(BattleMgr.selectedCardOrder[i]);
                break;
            }
        }
        BattleMgr.selectedCardDic.Remove(cardKey);  //선택된 카드 딕셔너리에서 제거
        GameObject obj = GameObject.Find("BattleMgr");
        BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
        battleMgr.RefreshSelectedCardArea(); //선택된카드 화면 새로고침

        //가방안의 카드 중 해당하는 카드 찾아서 isSelected = false 해주기
        obj = GameObject.Find("BagContent");
        CardMgr[] childList = obj.GetComponentsInChildren<CardMgr>();
        for(int i = 1; i < childList.Length; i++)
        {
            if (childList[i].cardKey == cardKey)
            {
                childList[i].isSelected = false;
            }
        }

        if(tag == "UtilCard")
        {
            GameMgr.tempSp -= cardSP;
            if (GameMgr.tempSp <= 0)
            {
                battleMgr.ClearSelectedCard();
                battleMgr.RefreshSelectedCardArea(); //선택된카드 화면 새로고침
                GameMgr.tempSp = GameMgr.curSp;
            }
        }
        else
        {
            GameMgr.tempSp += cardSP;
        }
    }
}
