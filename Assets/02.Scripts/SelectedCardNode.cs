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
        //���õ� ī�� ���� ����Ʈ���� key���� ��ġ�ϴ� ī�� ã�Ƽ� ����
        for(int i = 0; i < BattleMgr.selectedCardOrder.Count; i++)
        {
            int key = BattleMgr.selectedCardOrder[i].GetComponent<CardMgr>().cardKey;
            if(key == cardKey)
            {
                BattleMgr.selectedCardOrder.Remove(BattleMgr.selectedCardOrder[i]);
                break;
            }
        }
        BattleMgr.selectedCardDic.Remove(cardKey);  //���õ� ī�� ��ųʸ����� ����
        GameObject obj = GameObject.Find("BattleMgr");
        BattleMgr battleMgr = obj.GetComponent<BattleMgr>();
        battleMgr.RefreshSelectedCardArea(); //���õ�ī�� ȭ�� ���ΰ�ħ

        //������� ī�� �� �ش��ϴ� ī�� ã�Ƽ� isSelected = false ���ֱ�
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
                battleMgr.RefreshSelectedCardArea(); //���õ�ī�� ȭ�� ���ΰ�ħ
                GameMgr.tempSp = GameMgr.curSp;
            }
        }
        else
        {
            GameMgr.tempSp += cardSP;
        }
    }
}
